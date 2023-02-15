# ProceduralTerrainGeneration

A procedural terrain generation tool for the project of "Wissenschaftliches Arbeiten 2" at the Furtwangen University.

## Description

This is a simulation of hydraulic erosion on a terrain in real-time. The algorithm simulates how water flows on the terrain and erodes the terrain based on various factors such as sediment capacity, gravity, and erosion and deposition rates. The erode function is the main function that simulates the erosion process.

The erosion simulation uses a set of parameters that control how the erosion process behaves. These parameters include ```numIterations``` which controls the number of erosion iterations to run, ```maxSteps``` which controls the maximum number of steps water can take in one iteration, ```inertia``` which controls how much velocity is carried over from the previous step, ```sedimentCapacityFactor``` which controls how much sediment water can carry, ```minSedimentCapacity``` which controls the minimum amount of sediment water can carry, ```gravity``` which controls the strength of gravity, ```evaporateSpeed``` which controls how fast water evaporates, ```depositSpeed``` which controls how fast sediment is deposited, and ```erodeSpeed``` which controls how fast sediment is eroded.

The erosion simulation starts by selecting a random position on the terrain and a random direction. It then simulates how water flows in that direction, eroding the terrain and depositing sediment as it goes. It repeats this process for a specified number of iterations, simulating how water flows across the entire terrain. The erosion simulation also includes some randomness to make the simulation more realistic.

Overall, the Erode function is a complex and computationally expensive function that simulates how water flows on a terrain and erodes it over time. It is an example of how real-world phenomena can be simulated in real-time using modern computer graphics techniques.

## Parametrization

In my scientific work, multiple combinations of the parameters are being examined to determine their expected effects on the terrain. The goal is to understand the relationships between the parameters and the changes that occur in the terrain as a result. These relationships are being analyzed through a performance analysis, which measures the effectiveness of each parameter combination in achieving the desired outcome. By examining a variety of combinations and analyzing their performance, we can gain a deeper understanding of the terrain and identify the most effective ways to achieve our goals.

The scientific work can be viewed [here](https://cloud.steinba.de/s/RsNBWzfYmRCrqGk).

## Example

|         Before         |  After 100k iterations |
|------------------------|------------------------|
|<img src="https://github.com/LeonSteinbach/ProceduralTerrainGeneration/blob/main/screenshots/erosion_1.png">| <img src="https://github.com/LeonSteinbach/ProceduralTerrainGeneration/blob/main/screenshots/erosion_2.png"> |
