import lgpio
import requests
import time

API_STATUS = "https://weatherdress-api.azurewebsites.net/api/motor/status"

# BCM pin numbers
BUKSER_PINS = [17, 27, 22, 23]  # Motor 1 - BOARD 11,13,15,16
SKO_PINS    = [24, 25, 8, 7]    # Motor 2 - BOARD 18,22,24,26
JAKKE_PINS  = [6, 13, 19, 26]   # Motor 3 - BOARD 31,33,35,37

BUKSER_OMDREJNING = 512
SKO_OMDREJNING    = 1024
JAKKE_OMDREJNING  = 512

BUKSER_POSITIONER = {
    "shorts":      64,
    "sweatbukser": 192,
    "jeans":       320,
    "regnbukser":  448,
}

SKO_POSITIONER = {
    "gummistøvler":  0,
    "sneakers":      128,
    "sandaler":      512,
    "vinterstøvler": 768,
}

JAKKE_POSITIONER = {
    "t-shirt":     64,
    "sweatshirt":  192,
    "regnjakke":   320,
    "flyverdragt": 448,
}

HALF_STEP = [
    [1, 0, 0, 0],
    [1, 1, 0, 0],
    [0, 1, 0, 0],
    [0, 1, 1, 0],
    [0, 0, 1, 0],
    [0, 0, 1, 1],
    [0, 0, 0, 1],
    [1, 0, 0, 1],
]

def kør_skridt(h, pins, steps, reverse=False):
    sekvens = HALF_STEP[::-1] if reverse else HALF_STEP
    for _ in range(steps):
        for trin in sekvens:
            for pin, val in zip(pins, trin):
                lgpio.gpio_write(h, pin, val)
            time.sleep(0.002)
    for pin in pins:
        lgpio.gpio_write(h, pin, 0)

def kør_motor(h, pins, steps, fuld_omdrejning, reverse=False, pause=3):
    if steps == 0:
        return
    kør_skridt(h, pins, steps, reverse)
    time.sleep(pause)
    kør_skridt(h, pins, fuld_omdrejning - steps, reverse)

def tjek_trigger():
    try:
        r = requests.get(API_STATUS, timeout=5)
        if r.ok:
            data = r.json()
            if data.get("triggered"):
                return data
    except Exception as e:
        print(f"API fejl: {e}")
    return None

def main():
    h = lgpio.gpiochip_open(0)
    for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
        for p in pins:
            lgpio.gpio_claim_output(h, p, 0)

    print("Klar — venter på signal fra WeatherDress...")
    try:
        while True:
            data = tjek_trigger()
            if data:
                jakke = (data.get("jacket") or "").lower()
                bukser = (data.get("pants")  or "").lower()
                sko    = (data.get("shoes")  or "").lower()
                print(f"Signal: jakke={jakke}, bukser={bukser}, sko={sko}")

                jakke_steps  = JAKKE_POSITIONER.get(jakke,  64)
                bukser_steps = BUKSER_POSITIONER.get(bukser, 64)
                sko_steps    = SKO_POSITIONER.get(sko,       128)

                kør_motor(h, BUKSER_PINS, steps=bukser_steps, fuld_omdrejning=BUKSER_OMDREJNING)
                kør_motor(h, SKO_PINS,    steps=sko_steps,    fuld_omdrejning=SKO_OMDREJNING)
                kør_motor(h, JAKKE_PINS,  steps=jakke_steps,  fuld_omdrejning=JAKKE_OMDREJNING, reverse=True)
                print("Færdig.")
            time.sleep(2)
    except KeyboardInterrupt:
        print("\nStoppet.")
    finally:
        for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
            for p in pins:
                lgpio.gpio_write(h, p, 0)
        lgpio.gpiochip_close(h)

if __name__ == "__main__":
    main()
