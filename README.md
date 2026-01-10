# Tanuki Sunset deep reinforcement learning agent
*Nov 2025 - Dec 2025*
This project features an autonomous agent trained using deep reinforcement learning (DRL) for a customized version of the game Tanuki Sunset. The agent serves as a real-time racing line assistant, helping players visualize and follow the most approximate efficient trajectory on the track.


## Repository contents

* TanukiAgent.cs: the core C# script that manages the agent logic, processes sensor data, and handles the reward system.
* Tanuki.yaml: the configuration file containing the hyperparameters used to train the agent via Unity ML-Agents.

## Agent architecture

### Perception (inputs)
The agent perceives its environment through a multi-layered sensor system:
* Ray Perception Sensor 3D: a 16-ray array detecting tags such as road, car, obstacle, and cone.
* Rigid body sensor: provides data regarding the agent's linear velocity and angular rotation.

### Actions (outputs)
The agent executes a hybrid control scheme:
* Continuous actions: steering control (left/right).
* Discrete actions: acceleration and drifting maneuvers.

### Rewards
The learning process was guided by the following incentives:
* Positive rewards: constant speed maintenance and controlled drifting.
* Negative rewards: penalties for collisions (-1.0), falling off the track (-1.0), and time-based penalties to encourage efficiency.

## Training performance
The model was trained for over 4.5 million steps using the PPO (Proximal Policy Optimization) algorithm, focusing on maximizing agent efficiency and behavioral stability.

<p align="center"><b>Environment / Cumulative reward</b></p>

![Environment_Cumulative Reward](https://github.com/user-attachments/assets/fb5141f1-ff45-40b6-98cd-65e87ab696e4)


### Cumulative reward evolution:
- Initial exploration (0 - 1.2M steps): Stable reward (~2.5) while learning basic environment rules.
- Rapid improvement (1.2M - 2.5M steps): Sharp increase to 16 as the model discovers the optimal strategy.
- Convergence and optimization (2.5M - 4.7M steps): Reward peaks at 21 with minor fluctuations.
- Terminal performance drop (~4.7M steps): Sudden decline likely due to parameter testing.

## Key features
* Autonomous navigation: successfully navigates complex tracks and avoids moving obstacles.
* Visual guidance: generates a dynamic path for the player to follow.
* Robustness: integrated with a respawn system to ensure the guidance line remains consistent throughout the race.

## Technologies used
* Unity
* ML-Agents toolkit
* C#
* TensorBoard

## Team members
* Gerardo Díaz López
* Fernando Medina Domínguez
* Mateo Segura Guerrero
