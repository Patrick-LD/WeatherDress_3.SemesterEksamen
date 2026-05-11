"""
WeatherDress — Raspberry Pi motor controller med Sense HAT
==========================================================
Dette script:
  1. Henter vejrdata fra vores WeatherDress API
  2. Viser vejret på Sense HAT displayet (8x8 LED matrix)
  3. Drejer 3 separate stepper motorer til den rigtige position:
       Motor 1 (venstre)  — Jakke-hjul
       Motor 2 (midten)   — Bukser-hjul
       Motor 3 (højre)    — Sko-hjul  (bruger OGSÅ gårsdagens vejr)

Hardware:
  3 x 28BYJ-48 stepper motor + ULN2003 driver board
  Raspberry Pi med Sense HAT
  VIGTIGT: Sense HAT sidder oven på Pi'en og dækker GPIO-headeren.
           Brug en GPIO pass-through HAT eller forlænger-header så
           du stadig kan bruge pins til motorerne.

GPIO pins (BOARD nummerering — de fysiske tal på stikket):
  Motor 1 Jakke  → pins 11, 13, 15, 16
  Motor 2 Bukser → pins 18, 22, 24, 26
  Motor 3 Sko    → pins 29, 31, 33, 35
  5V til motorer → pin 2  (ekstern 5V adapter — IKKE Pi'ens egen 5V)
  GND            → pin 6
"""

# ==============================================================================
# IMPORTS — biblioteker vi bruger
# ==============================================================================
# RPi.GPIO styrer de fysiske pins på Pi'en (til motorerne)
import RPi.GPIO as GPIO

# SenseHat styrer displayet og LED-matricen
from sense_hat import SenseHat

# urllib.request bruges til at kalde vores API over netværket
import urllib.request

# json bruges til at læse svaret fra API'en (det kommer som JSON tekst)
import json

# time bruges til pauser — f.eks. time.sleep(0.5) venter et halvt sekund
import time

# sys bruges til at stoppe programmet med en fejlbesked
import sys


# ==============================================================================
# KONFIGURATION — ændr disse to linjer til jeres egne værdier
# ==============================================================================
API_BASE_URL = "http://<din-server-ip>:5058"   # IP-adressen hvor API'en kører
ZIP_CODE = "2100"                               # Postnummer der slås op

# Hastighed på motorerne — 0.002 sekunder mellem hvert skridt
# Gør tallet STØRRE hvis motoren mister skridt (går i stå eller hakker)
STEP_DELAY = 0.002


# ==============================================================================
# GPIO PINS — fra hardware-guiden (BOARD nummerering)
# ==============================================================================
# Vi bruger GPIO.BOARD — det betyder vi bruger de fysiske pin-numre
# som sidder trykt på Pi'ens GPIO-stik (1-40), IKKE BCM-numrene

JAKKE_PINS  = [11, 13, 15, 16]   # Motor 1 — Jakke-hjul  (venstre)
BUKSER_PINS = [18, 22, 24, 26]   # Motor 2 — Bukser-hjul (midten)
SKO_PINS    = [29, 31, 33, 35]   # Motor 3 — Sko-hjul    (højre)

# Alle tre motorer samlet i en liste — nyttigt når vi sætter dem op
ALLE_MOTORER = [JAKKE_PINS, BUKSER_PINS, SKO_PINS]


# ==============================================================================
# MOTOR KONSTANTER
# ==============================================================================
# 28BYJ-48 med gearbox drejer 2048 skridt per fuld omdrejning (360°)
# Vi har 4 positioner = 4 x 90° — hvert 90° = 2048 / 4 = 512 skridt
STEPS_PER_REVOLUTION = 2048
STEPS_PER_90_DEGREES = STEPS_PER_REVOLUTION // 4   # = 512

# Halv-skridt sekvens — 8 trin der fortæller hvilke pins der skal tændes
# Hvert tal er enten 1 (tænd pin) eller 0 (sluk pin)
# Rækkefølgen [IN1, IN2, IN3, IN4] svarer til pin-rækkefølgen i JAKKE_PINS etc.
STEP_SEQUENCE = [
    [1, 0, 0, 0],
    [1, 1, 0, 0],
    [0, 1, 0, 0],
    [0, 1, 1, 0],
    [0, 0, 1, 0],
    [0, 0, 1, 1],
    [0, 0, 0, 1],
    [1, 0, 0, 1],
]


# ==============================================================================
# SENSE HAT IKONER (8x8 pixels)
# ==============================================================================
# Sense HAT har et 8x8 LED display = 64 prikker
# Vi definerer farver som (rød, grøn, blå) — tal fra 0 til 255
# Eksempel: (255, 0, 0) = rød, (0, 255, 0) = grøn, (0, 0, 0) = sort (slukket)

K  = (  0,   0,   0)   # Sort  — slukket
Y  = (255, 200,   0)   # Gul   — sol
O  = (255, 140,   0)   # Orange — sol stråler
B  = (  0,  80, 200)   # Blå   — regn
LB = (100, 150, 255)   # Lyseblå — regndråber
G  = (130, 130, 130)   # Grå   — overskyet
W  = (200, 210, 255)   # Hvid/blå — sne

# Sol-ikon: gul cirkel med orange stråler
SOL_IKON = [
    K, K, K, Y, Y, K, K, K,
    K, O, K, Y, Y, K, O, K,
    K, K, O, O, O, O, K, K,
    Y, Y, O, Y, Y, O, Y, Y,
    Y, Y, O, Y, Y, O, Y, Y,
    K, K, O, O, O, O, K, K,
    K, O, K, Y, Y, K, O, K,
    K, K, K, Y, Y, K, K, K,
]

# Sky-ikon: grå sky
OVERSKYET_IKON = [
    K, K, G, G, G, K, K, K,
    K, G, G, G, G, G, K, K,
    G, G, G, G, G, G, G, K,
    G, G, G, G, G, G, G, G,
    G, G, G, G, G, G, G, G,
    K, G, G, G, G, G, K, K,
    K, K, K, K, K, K, K, K,
    K, K, K, K, K, K, K, K,
]

# Regn-ikon: grå sky med blå dråber under
REGN_IKON = [
    K,  K,  G,  G,  G,  K,  K,  K,
    K,  G,  G,  G,  G,  G,  K,  K,
    G,  G,  G,  G,  G,  G,  G,  G,
    K,  K,  K,  K,  K,  K,  K,  K,
    K,  LB, K,  LB, K,  LB, K,  LB,
    LB, K,  LB, K,  LB, K,  LB, K,
    K,  B,  K,  B,  K,  B,  K,  B,
    B,  K,  B,  K,  B,  K,  B,  K,
]

# Sne-ikon: hvide snefnug
SNE_IKON = [
    K, W, K, K, W, K, K, W,
    K, K, W, W, W, W, K, K,
    W, W, W, W, W, W, W, W,
    K, K, W, W, W, W, K, K,
    K, W, K, K, W, K, K, W,
    K, K, K, K, K, K, K, K,
    W, K, W, K, W, K, W, K,
    K, W, K, W, K, W, K, W,
]

# Ordbog der kobler vejr-kategori til det rigtige ikon
IKONER = {
    "Sol/varmt":  SOL_IKON,
    "Overskyet":  OVERSKYET_IKON,
    "Regn":       REGN_IKON,
    "Koldt/sne":  SNE_IKON,
}


# ==============================================================================
# GPIO OPSÆTNING OG MOTOR FUNKTIONER
# ==============================================================================

def setup_gpio():
    """Sætter alle GPIO pins op som output og slukker dem."""
    GPIO.setmode(GPIO.BOARD)   # Vi bruger BOARD numre (fysiske pin-numre)
    GPIO.setwarnings(False)    # Sluk advarsler hvis pins allerede er sat op

    # Løkke der går igennem alle 3 motorer og deres 4 pins
    for motor_pins in ALLE_MOTORER:
        for pin in motor_pins:
            GPIO.setup(pin, GPIO.OUT)   # Sæt pin som output (sender signal)
            GPIO.output(pin, 0)         # Sluk pin (0 = slukket, 1 = tændt)


def sluk_motor(motor_pins):
    """Slukker alle 4 pins på en motor så den ikke overophedes."""
    for pin in motor_pins:
        GPIO.output(pin, 0)


def cleanup_gpio():
    """Slukker alle pins og frigiver GPIO. Kaldes altid til sidst."""
    for motor_pins in ALLE_MOTORER:
        sluk_motor(motor_pins)
    GPIO.cleanup()


def drej_motor(motor_pins, antal_skridt, med_uret=True):
    """
    Drejer én motor et bestemt antal skridt.

    motor_pins  — liste med de 4 GPIO pins til denne motor
    antal_skridt — hvor mange skridt motoren skal tage
    med_uret    — True = med uret, False = mod uret
    """
    # Vend sekvensen om hvis vi skal dreje mod uret
    sekvens = STEP_SEQUENCE if med_uret else list(reversed(STEP_SEQUENCE))

    # Gentag sekvensen 'antal_skridt' gange
    for _ in range(antal_skridt):
        for trin in sekvens:
            # Sæt hvert pin til den rigtige tilstand (1 eller 0)
            for i, pin in enumerate(motor_pins):
                GPIO.output(pin, trin[i])
            time.sleep(STEP_DELAY)   # Kort pause mellem hvert skridt


def nulstil_motor(motor_pins, navn):
    """Drejer motoren en hel omgang mod uret for at komme tilbage til position 0."""
    print(f"  Nulstiller {navn}-hjul til position 0...")
    drej_motor(motor_pins, STEPS_PER_REVOLUTION, med_uret=False)
    sluk_motor(motor_pins)   # Sluk pins mens vi venter
    time.sleep(0.3)          # Kort pause inden næste motor


def drej_til_vinkel(motor_pins, vinkel, navn):
    """Drejer motoren fra 0° til den ønskede vinkel med uret."""
    # Beregn antal skridt: 90° = 512 skridt, 180° = 1024 skridt, 270° = 1536 skridt
    antal_skridt = int(STEPS_PER_90_DEGREES * (vinkel / 90))
    print(f"  {navn}: drejer til {vinkel}° ({antal_skridt} skridt)...")
    drej_motor(motor_pins, antal_skridt, med_uret=True)
    sluk_motor(motor_pins)   # Sluk pins når motoren er på plads


# ==============================================================================
# SENSE HAT FUNKTIONER
# ==============================================================================

def vis_vejr_ikon(sense, vejr_kategori):
    """Viser vejr-ikon på Sense HAT's 8x8 LED display."""
    # Hent det rigtige ikon — brug sol-ikonet hvis kategorien ikke kendes
    ikon = IKONER.get(vejr_kategori, SOL_IKON)
    sense.set_pixels(ikon)


def vis_tekst(sense, tekst, farve=(255, 255, 255)):
    """Scroller tekst hen over Sense HAT displayet."""
    # scroll_speed: lavere tal = langsommere (0.08 er en god hastighed)
    sense.show_message(tekst, scroll_speed=0.08, text_colour=farve)


def vis_opstart(sense):
    """Viser velkomst-animation på Sense HAT."""
    # Blå farve = (0, 150, 255)
    vis_tekst(sense, "WeatherDress", farve=(0, 150, 255))
    sense.clear()


# ==============================================================================
# API KALD
# ==============================================================================

def hent_toj_anbefaling(postnummer):
    """
    Kalder WeatherDress API og returnerer tøjanbefaling.
    Returnerer en dictionary med alle felter fra API-svaret.
    """
    url = f"{API_BASE_URL}/api/weatherforecast/{postnummer}/clothing-position"
    print(f"Henter vejrdata: {url}")

    try:
        # urllib.request.urlopen åbner URL'en — timeout=10 giver op efter 10 sekunder
        with urllib.request.urlopen(url, timeout=10) as svar:
            # Læs svaret og konverter fra JSON-tekst til Python dictionary
            data = json.loads(svar.read().decode())
            return data
    except Exception as fejl:
        print(f"FEJL ved API-kald: {fejl}")
        print("Tjek at API'en kører og at API_BASE_URL er korrekt.")
        sys.exit(1)   # Stop programmet


# ==============================================================================
# HOVED-PROGRAM
# ==============================================================================

def main():
    print("=" * 50)
    print("WeatherDress Motor Controller")
    print("=" * 50)

    # --- Start Sense HAT ---
    sense = SenseHat()
    sense.clear()
    vis_opstart(sense)

    # --- Hent vejrdata fra API ---
    print("\n[1/3] Henter vejrdata fra API...")
    anbefaling = hent_toj_anbefaling(ZIP_CODE)

    # Udskriv hvad vi fik fra API'en
    print(f"\nBy:      {anbefaling['location']}")
    print(f"Vejr:    {anbefaling['currentDescription']} ({anbefaling['currentTemperatureC']}°C)")
    print(f"Kategori: {anbefaling['weatherCategory']}")
    print(f"\nAnbefaling:")
    print(f"  Jakke:  {anbefaling['jacket']}  → Motor 1: {anbefaling['jacketMotorAngle']}°")
    print(f"  Bukser: {anbefaling['pants']}  → Motor 2: {anbefaling['pantsMotorAngle']}°")
    print(f"  Sko:    {anbefaling['shoes']} → Motor 3: {anbefaling['shoesMotorAngle']}°")

    # --- Vis vejr-ikon på Sense HAT ---
    vis_vejr_ikon(sense, anbefaling['weatherCategory'])

    # --- Sæt GPIO pins op ---
    print("\n[2/3] Sætter GPIO pins op...")
    setup_gpio()

    try:
        # --- Nulstil alle 3 motorer til position 0 ---
        print("\nNulstiller alle hjul til position 0...")
        nulstil_motor(JAKKE_PINS,  "Jakke")
        nulstil_motor(BUKSER_PINS, "Bukser")
        nulstil_motor(SKO_PINS,    "Sko")

        print("\n[3/3] Drejer hjulene til rigtig position...")

        # --- Drej Motor 1: Jakke-hjul ---
        drej_til_vinkel(JAKKE_PINS, anbefaling['jacketMotorAngle'], "Jakke")

        # --- Drej Motor 2: Bukser-hjul ---
        drej_til_vinkel(BUKSER_PINS, anbefaling['pantsMotorAngle'], "Bukser")

        # --- Drej Motor 3: Sko-hjul ---
        # OBS: Sko-vinklen kan være FORSKELLIG fra jakke/bukser
        # fordi den også tager hensyn til gårsdagens vejr
        drej_til_vinkel(SKO_PINS, anbefaling['shoesMotorAngle'], "Sko")

        print("\nAlle hjul er på plads!")

        # --- Vis tøjanbefalingen på Sense HAT ---
        tekst = f"{anbefaling['jacket']} / {anbefaling['pants']} / {anbefaling['shoes']}"
        vis_tekst(sense, tekst, farve=(0, 255, 100))

        # Hold vejr-ikonet fremme i 5 sekunder
        vis_vejr_ikon(sense, anbefaling['weatherCategory'])
        time.sleep(5)

    finally:
        # 'finally' kører ALTID — også hvis der sker en fejl
        # Det sikrer at pins altid bliver slukket
        cleanup_gpio()
        sense.clear()
        print("\nFærdig. GPIO ryddet op.")


# Denne linje sikrer at main() kun køres hvis vi starter filen direkte
# (ikke hvis en anden fil importerer dette script)
if __name__ == "__main__":
    main()
