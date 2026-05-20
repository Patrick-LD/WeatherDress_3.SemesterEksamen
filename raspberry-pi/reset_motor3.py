import lgpio
import time

# BCM pins for Motor 3 Sko (BOARD 31,33,35,37)
PINS = [6, 13, 19, 26]

HALF_STEP = [
    [1,0,0,0],[1,1,0,0],[0,1,0,0],[0,1,1,0],
    [0,0,1,0],[0,0,1,1],[0,0,0,1],[1,0,0,1],
]

h = lgpio.gpiochip_open(0)
for p in PINS:
    lgpio.gpio_claim_output(h, p, 0)

try:
    sekvens = HALF_STEP[::-1]
    for _ in range(384):
        for trin in sekvens:
            for pin, val in zip(PINS, trin):
                lgpio.gpio_write(h, pin, val)
            time.sleep(0.002)
finally:
    for p in PINS:
        lgpio.gpio_write(h, p, 0)
    lgpio.gpiochip_close(h)

print("Faerdig")
