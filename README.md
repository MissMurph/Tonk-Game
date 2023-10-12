# Tonk-Game

Tonk game is a game about managing a World War 2 tank crew, combining both strategy controls & systems with direct control and interaction with the playspace to make a strategy action hybrid. Inspired by the game 'FTL: Faster Than Light' and the movie 'Fury', it was envisioned to be a rogue-lite game about sacrifice, stress and tenacity in war.

![]()

## Unity

Version `2021.3.6f1`

## Key Features

### Autonomous AI & Statemachine
The game uses a Pawn system inspired by Rimworld, where characters behave autonomously using goal driven weighted decision making. It supports complex decision trees built within goals that the Statemachine weighs against each other to determine the best decision to make in that moment. It uses multi threaded code (however no thread pooling yet) to optimize the decision making, resulting in minimal overhead for the actual game. AI is capable of understanding when it's in danger and to interrupt its current task to protect itself.

![]()

### Hybrid Gameplay
The game defines two modes of gameplay to marry together direct control of the tank and strategy controls of the characters; Implicit & Explicit. With your characters boarded in a tank, switch to Explicit to directly control the tank when under stress while the character's AI will support the tank and kep it operating. When not in battle, use Implicit to strategize and prepare, commanding characters to work managing inventories, repairing and looting buildings.

![]()

### Interaction System
All interactions between AI & world objects is serialized with multiple stages & results which allows the AI to create decisions with. Interactions handle everything between entering a tank, repairing a system, healing a character, looting a cache and placing an item in an inventory. Interactions even support pre-requisites, which the AI will complete before attempting to perform the task, such as grabbing a required item from an inventory.

![]()

### Inventory System
The inventory system is driven by the interaction system, having minimum requirements to complete in order for any inventory transaction to be complete. As a result, the game supports moving items between different dimensions of inventory and currently has implemented both a 1 dimensional inventory & a 2 dimensional inventory where items have different sizes. Thanks to the simplicity of items & the power of the interaction system, there is no translation needed, the inventories implicitly understand how to move items between each other.

![]()

## Challenges Faced



## What I've Learnt