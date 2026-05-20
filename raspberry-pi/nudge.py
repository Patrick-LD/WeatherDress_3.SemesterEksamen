import lgpio
import time

BUKSER_PINS = [17, 27, 22, 23]
SKO_PINS    = [6, 13, 19, 26]
JAKKE_PINS  = [24, 25, 8, 7]

HALF_STEP = [
    [1,0,0,0],[1,1,0,0],[0,1,0,0],[0,1,1,0],
    [0,0,1,0],[0,0,1,1],[0,0,0,1],[1,0,0,1],
]

h = lgpio.gpiochip_open(0)
for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
    for p in pins:
        lgpio.gpio_claim_output(h, p, 0)

def kør(pins, steps, reverse=False):
    seq = HALF_STEP[::-1] if reverse else HALF_STEP
    for _ in range(steps):
        for trin in seq:
            for pin, val in zip(pins, trin):
                lgpio.gpio_write(h, pin, val)
            time.sleep(0.002)
    for p in pins:
        lgpio.gpio_write(h, p, 0)

print("Tryk: 1=Bukser  2=Sko  3=Jakke  q=Afslut")
print("Korer 64 skridt frem for valgt motor.")

try:
    while True:
        valg = input("> ").strip()
        if valg == "1":
            kør(BUKSER_PINS, 64)
            print("Bukser: 64 skridt")
        elif valg == "2":
            kør(SKO_PINS, 64)
            print("Sko: 64 skridt")
        elif valg == "3":
            kør(JAKKE_PINS, 64, reverse=True)
            print("Jakke: 64 skridt")
        elif valg == "q":
            break
        else:
            print("Ugyldigt valg")
finally:
    for pins in [BUKSER_PINS, SKO_PINS, JAKKE_PINS]:
        for p in pins:
            lgpio.gpio_write(h, p, 0)
    lgpio.gpiochip_close(h)
