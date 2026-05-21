import lgpio
import requests
import time

API_STATUS = "https://weatherdress-api.azurewebsites.net/api/motor/status"

# BCM pin numbers
BUKSER_PINS = [17, 27, 22, 23]  # Motor 1 - BOARD 11,13,15,16
SKO_PINS    = [24, 25, 8, 7]    # Motor 2 - BOARD 18,22,24,26
JAKKE_PINS  = [6, 13, 19, 26]   # Motor 3 - BOARD 31,33,35,37

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

def kør_skridt(h, pins, steps, reverse=False):
    sekvens = HALF_STEP[::-1] if reverse else HALF_STEP
    for _ in range(steps):
        for trin in sekvens:
            for pin, val in zip(pins, trin):
                lgpio.gpio_write(h, pin, val)
            time.sleep(0.002)
    for pin in pins:
        lgpio.gpio_write(h, pin, 0)

def kør_motor(h, pins, steps=64, reverse=False, pause=3):
    kør_skridt(h, pins, steps, reverse)
    time.sleep(pause)
    kør_skridt(h, pins, FULD_OMDREJNING - steps, reverse)

def tjek_trigger():
    try:
        r = requests.get(API_STATUS, timeout=5)
        if r.ok:
            return r.json().get("triggered", False)
    except Exception as e:
        print(f"API fejl: {e}")
    return False

def main():
    h = lgpio.gpiochip_open(0)
    for pins in [BUKSER_PINS, JAKKE_PINS, SKO_PINS]:
        for p in pins:
            lgpio.gpio_claim_output(h, p, 0)

    print("Klar — venter på signal fra WeatherDress...")
    try:
        while True:
            if tjek_trigger():
                print("Signal modtaget! Starter motorer...")
                kør_motor(h, BUKSER_PINS, steps=64)
                kør_motor(h, SKO_PINS, steps=64)
                kør_motor(h, JAKKE_PINS, steps=64, reverse=True)
                print("Færdig.")
            time.sleep(2)
    except KeyboardInterrupt:
        print("\nStoppet.")
    finally:
        for pins in [BUKSER_PINS, JAKKE_PINS, SKO_PINS]:
            for p in pins:
                lgpio.gpio_write(h, p, 0)
        lgpio.gpiochip_close(h)

if __name__ == "__main__":
    main()
