import RPi.GPIO as GPIO
import requests
import time

API_STATUS = "https://weatherdress-api.azurewebsites.net/api/motor/status"

JAKKE_PINS  = [11, 13, 15, 16]
BUKSER_PINS = [18, 22, 24, 26]
SKO_PINS    = [29, 31, 33, 35]

FULD_OMDREJNING = 512

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

def setup():
    GPIO.setmode(GPIO.BOARD)
    for pins in [JAKKE_PINS, BUKSER_PINS, SKO_PINS]:
        for pin in pins:
            GPIO.setup(pin, GPIO.OUT)
            GPIO.output(pin, GPIO.LOW)

def _kør_skridt(pins, steps, sekvens, delay):
    for _ in range(steps):
        for trin in sekvens:
            for pin, val in zip(pins, trin):
                GPIO.output(pin, GPIO.HIGH if val else GPIO.LOW)
            time.sleep(delay)
    for pin in pins:
        GPIO.output(pin, GPIO.LOW)

def kør_motor(pins, steps=512, delay=0.002, reverse=False, pause=3):
    sekvens = HALF_STEP[::-1] if reverse else HALF_STEP
    retur_skridt = FULD_OMDREJNING - steps

    _kør_skridt(pins, steps, sekvens, delay)
    time.sleep(pause)
    _kør_skridt(pins, retur_skridt, sekvens, delay)

def tjek_trigger():
    try:
        r = requests.get(API_STATUS, timeout=5)
        if r.ok:
            return r.json().get("triggered", False)
    except Exception as e:
        print(f"API fejl: {e}")
    return False

def main():
    setup()
    print("Klar — venter på signal fra WeatherDress...")
    try:
        while True:
            if tjek_trigger():
                print("Signal modtaget! Starter motorer...")
                kør_motor(JAKKE_PINS)
                kør_motor(BUKSER_PINS)
                kør_motor(SKO_PINS, reverse=True)
                print("Færdig.")
            time.sleep(2)
    except KeyboardInterrupt:
        print("\nStoppet.")
    finally:
        GPIO.cleanup()

if __name__ == "__main__":
    main()
