import lgpio
import time

BUKSER_PINS = [17, 27, 22, 23]  # Motor 1 - BOARD 11,13,15,16
SKO_PINS    = [24, 25, 8, 7]    # Motor 2 - BOARD 18,22,24,26
JAKKE_PINS  = [6, 13, 19, 26]   # Motor 3 - BOARD 31,33,35,37

BUKSER_OMDREJNING = 512
SKO_OMDREJNING    = 1024
JAKKE_OMDREJNING  = 512

BUKSER_POSITIONER = [
    ("shorts",      64),
    ("sweatbukser", 192),
    ("jeans",       320),
    ("regnbukser",  448),
]

SKO_POSITIONER = [
    ("gummistøvler",  0),
    ("sneakers",      128),
    ("sandaler",      512),
    ("vinterstøvler", 768),
]

JAKKE_POSITIONER = [
    ("t-shirt",     64),
    ("sweatshirt",  192),
    ("regnjakke",   320),
    ("flyverdragt", 448),
]

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

def test_motor(h, navn, pins, positioner, fuld_omdrejning, reverse=False):
    print(f"\n=== {navn} ===")
    print("Sørg for at pilen er på HOME, tryk Enter for at starte.")
    input()

    forrige = 0
    for navn_pos, steps in positioner:
        if steps == 0:
            print(f"  Position 0 (home) = '{navn_pos}' — ingen bevægelse")
            input("Passer det? Tryk Enter for næste...")
            continue

        flyt = steps - forrige
        print(f"Kører {flyt} skridt til position {steps} ({navn_pos})...")
        kør_skridt(h, pins, flyt, reverse)
        ok = input(f"  Peger pilen på '{navn_pos}'? (Enter=ja / skriv nej): ").strip().lower()
        if ok == "nej":
            print(f"  !! FEJL ved '{navn_pos}' på {steps} skridt")
        forrige = steps

    print("Kører tilbage til home...")
    kør_skridt(h, pins, fuld_omdrejning - forrige, reverse)
    print(f"{navn} færdig.\n")

def main():
    h = lgpio.gpiochip_open(0)
    for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
        for p in pins:
            lgpio.gpio_claim_output(h, p, 0)

    try:
        test_motor(h, "BUKSER (Motor 1)", BUKSER_PINS, BUKSER_POSITIONER, BUKSER_OMDREJNING)
        test_motor(h, "SKO (Motor 2)",    SKO_PINS,    SKO_POSITIONER,    SKO_OMDREJNING)
        test_motor(h, "JAKKE (Motor 3)",  JAKKE_PINS,  JAKKE_POSITIONER,  JAKKE_OMDREJNING, reverse=True)
        print("Alle motorer testet!")
    except KeyboardInterrupt:
        print("\nAfbrudt.")
    finally:
        for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
            for p in pins:
                lgpio.gpio_write(h, p, 0)
        lgpio.gpiochip_close(h)

if __name__ == "__main__":
    main()
