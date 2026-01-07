# Tanuki Sunset deep reinforcement learning agent

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
The model was trained for 4.5 million steps using the PPO (Proximal Policy Optimization) algorithm. The training focused on maximizing agent efficiency and behavioral stability.

<img width="2352" height="1333" alt="Untitled-2025-11-21-1940" src="https://github.com/user-attachments/assets/26d158dc-f087-42a2-882f-c82bafbc2edb" />

The cumulative reward graph shows a clear three-stage learning process:
- Initial Exploration (0 - 1.2M steps): The agent maintains a low reward (approx. 2.5) while learning basic environment rules.
- Rapid Improvement (1.2M - 2.5M steps): A sharp upward trend as the model discovers the optimal strategy, jumping from 2.5 to 16.
- Convergence (2.5M - 4.5M steps): The model stabilizes, reaching a peak reward of approximately 21. Despite minor fluctuations, the consistent high values indicate a successful and stable training outcome.

## Key features
* Autonomous navigation: successfully navigates complex tracks and avoids moving obstacles.
* Visual guidance: generates a dynamic path for the player to follow.
* Robustness: integrated with a respawn system to ensure the guidance line remains consistent throughout the race.

## Technologies used
* Unity
* ML-Agents toolkit
* C#
* TensorBoard

## Members
* Gerardo Diaz Lopez A01425577
* Fernando Medina Dominguez A01425797
* Mateo Segura Guerrero A01425792
