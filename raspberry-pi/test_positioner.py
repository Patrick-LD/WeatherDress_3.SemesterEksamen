import RPi.GPIO as GPIO
import time

BUKSER_PINS  = [11, 13, 15, 16]
JAKKE_PINS   = [18, 22, 24, 26]
SKO_PINS     = [29, 31, 33, 35]

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
    for pins in [BUKSER_PINS, JAKKE_PINS, SKO_PINS]:
        for pin in pins:
            GPIO.setup(pin, GPIO.OUT)
            GPIO.output(pin, GPIO.LOW)

def kør_skridt(pins, steps, reverse=False):
    sekvens = HALF_STEP[::-1] if reverse else HALF_STEP
    for _ in range(steps):
        for trin in sekvens:
            for pin, val in zip(pins, trin):
                GPIO.output(pin, GPIO.HIGH if val else GPIO.LOW)
            time.sleep(0.002)
    for pin in pins:
        GPIO.output(pin, GPIO.LOW)

def test_motor(navn, pins, reverse=False):
    print(f"\n=== {navn} ===")
    print("Sørg for at pilen er på HOME inden du trykker Enter.")
    input("Tryk Enter for at starte...")

    for pos in [64, 192, 320, 448]:
        print(f"\nKører til position {pos} skridt...")
        kør_skridt(pins, pos, reverse)
        item = input(f"Hvad peger pilen på ved {pos} skridt? Skriv her: ")
        print(f"  → Position {pos}: {item}")
        print("Kører tilbage til home...")
        kør_skridt(pins, FULD_OMDREJNING - pos, reverse)
        time.sleep(1)

    print(f"\n{navn} færdig.")

def main():
    setup()
    try:
        test_motor("BUKSER (Motor 1)", BUKSER_PINS, reverse=False)
        test_motor("JAKKE/OVERTØJ (Motor 2)", JAKKE_PINS, reverse=False)
        test_motor("SKO/FODTØJ (Motor 3)", SKO_PINS, reverse=True)
        print("\nAlle motorer testet. Noter dine resultater.")
    except KeyboardInterrupt:
        print("\nAfbrudt.")
    finally:
        GPIO.cleanup()

if __name__ == "__main__":
    main()
