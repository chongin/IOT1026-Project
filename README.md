<p align="center">
	<a href="https://github.com/chongin/IOT1026-Project/actions/workflows/ci.yml">
    <img src="https://github.com/chongin/IOT1026-Project/actions/workflows/ci.yml/badge.svg"/>
    </a>
	<a href="https://github.com/chongin/IOT1026-Project/actions/workflows/formatting.yml">
    <img src="https://github.com/chongin/IOT1026-Project/actions/workflows/formatting.yml/badge.svg"/>
	<br/>
    <a href="https://codecov.io/gh/chongin/IOT1026-Project" > 
    <img src="https://codecov.io/gh/chongin/IOT1026-Project/branch/main/graph/badge.svg?token=JS0857X5JD"/> 
	<img title="MIT License" alt="license" src="https://img.shields.io/badge/license-MIT-informational?style=flat-square">	
    </a>
</p>

# IOT1026-Project
Write a description of your `Room` and `Monster` class here.

[Assignment Instructions](docs/instructions.md)  
[How to start coding](docs/how-to-use.md)  
[How to update status badges](docs/how-to-update-badges.md)

[![ci](https://github.com/chongin/IOT1026-Project/actions/workflows/ci.yml/badge.svg)](https://github.com/chongin/IOT1026-Project/actions/workflows/ci.yml)

[![formatting](https://github.com/chongin/IOT1026-Project/actions/workflows/formatting.yml/badge.svg)](https://github.com/chongin/IOT1026-Project/actions/workflows/formatting.yml)


# Add Toxic room summary
I add a toxic room. When hero walk into this room, he will play a dice game. If he win, nothing will happen, if he lost, he can at most play n(config) more steps, then he will die. He should leave the entrance before n steps.


# Add Monstor summary
I designed a monster named Fire Dragon, which destroys the maze room, traps the hero, and eventually dies. It can destroy empty rooms and feature rooms that are no longer active. If a hero encounters a destroyed room, he/she cannot move to that room, but Monsters can travel through destroyed rooms. And monsters can copy themselves, speeding up the speed of destroying rooms.  If the hero cannot exit before all rooms are destroyed or he/she is surrounded by fire, he/she loses the game.
