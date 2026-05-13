"""
WeatherDress — Test af én motor (uden Sense HAT)
================================================
Brug dette script når du kun har tilsluttet én motor.
Scriptet drejer motoren:
  90° med uret → venter → tilbage til 0°
  180° med uret → venter → tilbage til 0°
  270° med uret → venter → tilbage til 0°

GPIO pins (BOARD nummerering — fysiske pin-numre på stikket):
  Motor 1 Jakke → pins 11, 13, 15, 16

Skift MOTOR_PINS hvis du bruger en anden motor:
  Bukser: [18, 22, 24, 26]
  Sko:    [29, 31, 33, 35]
"""

import RPi.GPIO as GPIO
import time

# Ændr disse pins til dem du faktisk har tilsluttet
MOTOR_PINS = [11, 13, 15, 16]

STEPS_PER_REVOLUTION = 2048
STEPS_PER_90_DEGREES = STEPS_PER_REVOLUTION // 4   # = 512

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
    for pin in MOTOR_PINS:
        GPIO.setup(pin, GPIO.OUT)
        GPIO.output(pin, 0)


def sluk():
    for pin in MOTOR_PINS:
        GPIO.output(pin, 0)


def drej(antal_skridt, med_uret=True):
    sekvens = STEP_SEQUENCE if med_uret else list(reversed(STEP_SEQUENCE))
    for _ in range(antal_skridt):
        for trin in sekvens:
            for i, pin in enumerate(MOTOR_PINS):
                GPIO.output(pin, trin[i])
            time.sleep(STEP_DELAY)


def test_vinkel(grader):
    antal_skridt = int(STEPS_PER_90_DEGREES * (grader / 90))
    print(f"\n  Drejer {grader}° med uret ({antal_skridt} skridt)...")
    drej(antal_skridt, med_uret=True)
    sluk()
    time.sleep(1)

    print(f"  Drejer {grader}° mod uret (tilbage til start)...")
    drej(antal_skridt, med_uret=False)
    sluk()
    time.sleep(0.5)
    print(f"  {grader}°: OK!")


def main():
    print("WeatherDress — Test af én motor")
    print(f"Pins: {MOTOR_PINS}\n")

    setup()

    try:
        test_vinkel(90)
        test_vinkel(180)
        test_vinkel(270)
        print("\nMotor test gennemført!")

    finally:
        sluk()
        GPIO.cleanup()
        print("GPIO ryddet op.")


if __name__ == "__main__":
    main()
