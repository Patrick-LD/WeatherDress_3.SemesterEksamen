"""
WeatherDress — Motor test script
=================================
Kør dette script FØR weather_motor.py for at tjekke at
alle 3 motorer virker korrekt.

Scriptet drejer:
  Motor 1 (Jakke)  90° → venter → tilbage til 0°
  Motor 2 (Bukser) 90° → venter → tilbage til 0°
  Motor 3 (Sko)    90° → venter → tilbage til 0°

Hvis et hjul drejer den forkerte vej — skift med_uret=True til False
i kaldene til drej_motor().
"""

import RPi.GPIO as GPIO
import time

# Samme pins som i weather_motor.py
JAKKE_PINS  = [11, 13, 15, 16]
BUKSER_PINS = [18, 22, 24, 26]
SKO_PINS    = [29, 31, 33, 35]

ALLE_MOTORER = [JAKKE_PINS, BUKSER_PINS, SKO_PINS]

STEPS_PER_REVOLUTION = 2048
STEPS_PER_90_DEGREES = STEPS_PER_REVOLUTION // 4

STEP_DELAY = 0.002

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


def setup():
    GPIO.setmode(GPIO.BOARD)
    GPIO.setwarnings(False)
    for motor_pins in ALLE_MOTORER:
        for pin in motor_pins:
            GPIO.setup(pin, GPIO.OUT)
            GPIO.output(pin, 0)


def sluk(motor_pins):
    for pin in motor_pins:
        GPIO.output(pin, 0)


def drej(motor_pins, antal_skridt, med_uret=True):
    sekvens = STEP_SEQUENCE if med_uret else list(reversed(STEP_SEQUENCE))
    for _ in range(antal_skridt):
        for trin in sekvens:
            for i, pin in enumerate(motor_pins):
                GPIO.output(pin, trin[i])
            time.sleep(STEP_DELAY)


def test_motor(motor_pins, navn):
    print(f"\nTester {navn}-motor...")
    print(f"  Drejer 90° med uret...")
    drej(motor_pins, STEPS_PER_90_DEGREES, med_uret=True)
    sluk(motor_pins)
    time.sleep(1)

    print(f"  Drejer 90° mod uret (tilbage til start)...")
    drej(motor_pins, STEPS_PER_90_DEGREES, med_uret=False)
    sluk(motor_pins)
    time.sleep(0.5)
    print(f"  {navn}: OK!")


def main():
    print("WeatherDress — Motor test")
    print("Tester alle 3 motorer — hver drejer 90° og tilbage\n")

    setup()

    try:
        test_motor(JAKKE_PINS,  "Jakke  (Motor 1)")
        test_motor(BUKSER_PINS, "Bukser (Motor 2)")
        test_motor(SKO_PINS,    "Sko    (Motor 3)")
        print("\nAlle motorer testet succesfuldt!")

    finally:
        for motor_pins in ALLE_MOTORER:
            sluk(motor_pins)
        GPIO.cleanup()
        print("GPIO ryddet op.")


if __name__ == "__main__":
    main()
