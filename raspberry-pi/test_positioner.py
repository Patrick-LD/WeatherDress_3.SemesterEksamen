import lgpio
import time

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

def test_motor(h, navn, pins, reverse=False):
    print(f"\n=== {navn} ===")
    print("Sørg for at pilen er på HOME inden du trykker Enter.")
    input("Tryk Enter for at starte...")

    for pos in [64, 192, 320, 448]:
        print(f"\nKører til position {pos} skridt...")
        kør_skridt(h, pins, pos, reverse)
        item = input(f"Hvad peger pilen på ved {pos} skridt? Skriv her: ")
        print(f"  → Position {pos}: {item}")
        print("Kører tilbage til home...")
        kør_skridt(h, pins, FULD_OMDREJNING - pos, reverse)
        time.sleep(1)

    print(f"\n{navn} færdig.")

def main():
    h = lgpio.gpiochip_open(0)
    for pins in [BUKSER_PINS, JAKKE_PINS, SKO_PINS]:
        for p in pins:
            lgpio.gpio_claim_output(h, p, 0)

    try:
        test_motor(h, "BUKSER (Motor 1)", BUKSER_PINS, reverse=False)
        test_motor(h, "SKO/FODTØJ (Motor 2)", SKO_PINS, reverse=False)
        test_motor(h, "JAKKE/OVERTØJ (Motor 3)", JAKKE_PINS, reverse=True)
        print("\nAlle motorer testet.")
    except KeyboardInterrupt:
        print("\nAfbrudt.")
    finally:
        for pins in [BUKSER_PINS, JAKKE_PINS, SKO_PINS]:
            for p in pins:
                lgpio.gpio_write(h, p, 0)
        lgpio.gpiochip_close(h)

if __name__ == "__main__":
    main()
