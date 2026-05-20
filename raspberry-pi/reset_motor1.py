import RPi.GPIO as GPIO
import time

PINS = [11, 13, 15, 16]
HS = [
    [1,0,0,0],[1,1,0,0],[0,1,0,0],[0,1,1,0],
    [0,0,1,0],[0,0,1,1],[0,0,0,1],[1,0,0,1],
]

GPIO.setmode(GPIO.BOARD)
for p in PINS:
    GPIO.setup(p, GPIO.OUT)
    GPIO.output(p, GPIO.LOW)

for _ in range(384):
    for trin in HS:
        for pin, val in zip(PINS, trin):
            GPIO.output(pin, GPIO.HIGH if val else GPIO.LOW)
        time.sleep(0.002)

for p in PINS:
    GPIO.output(p, GPIO.LOW)
GPIO.cleanup()
print("Faerdig")
