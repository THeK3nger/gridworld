GridWorld
=========

An 3D environment for testing AIs powered by [Unity3D][1]. It includes a general NPC architecture "framework". The goal is to make the creation of NPC characters very straightforward. 

Requisites
----------

This software is built on the free version of Unity3D 3.5.7. You have to use 
this version in order to run and edit it.

All the (free) plugins are included in the repository.

Linux Support
-------------

Also if this software is built on the 3.x version of Unity, it can be loaded 
with Unity 4 in order to use the Linux exporter. 

I'm trying to use a portable syntax avoiding all the deprecated functions. If
you find some problems in running GridWorld on Unity 4, plese, let me know.

The GoldRun Game
----------------

GridWorld implement a test game called GoldRun. 

The goal of the game is to take more gold as possible to the spawn point in a certain time limit (4 minutes default).

There are two things that make the game interesting:

 * More gold you carry, more slow you walk.
 * If the opponent chases you, you are destroyed and you respawn after 20 seconds (to be defined).
 * Power up and doors (to be implemented). You can unlock or lock
 shortcuts, slow your enemies, boost your speed and so on.

 This can also used to evaluate the AI power. :)

Developer Notes
---------------

During the development of this software I'm using [this branching method][2].


[1]: http://unity3d.com/
[2]: http://nvie.com/posts/a-successful-git-branching-model/
