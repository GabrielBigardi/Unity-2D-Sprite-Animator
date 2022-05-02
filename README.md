# 2D Sprite Animator for Unity Engine
 Tired of the "Unity Animator Hell", want more performance and faster workflow ? You've gone into the right place, this is a project i've made to replace Unity Animator usage on 2D projects, it's more performatic, simple and easy to use.

## How to install
### Package Manager (recommended)
* 1 - Open the package manager (Window > Package Manager).  
* 2 - Click on the plus icon and "Add package from git URL...".  
* 3 - Enter https://github.com/GabrielBigardi/2D-Sprite-Animator.git and click "Add".  
* 4 - Wait until the package manager finishes installing the package and recompiling.  
   
### Lazy way
* 1 - Download this repository as ZIP or by cloning it.
* 2 - Drag it into your "Assets" folder.
  
## Why to use
### Escaping Unity's Animator Hell
* Unity's Animator was made for 3D games, it has a lot of unuseful interpolation settings and it's a hell to manage.
* Unity's Animator is not fast to setup, you need to create animations, save it on a folder, setup transitions/parameters, try to organize the Animator window, etc...
### Performance
* Unity's Animator is pretty expensive for simple 2D games, more about that in the [benchmarking](#benchmarking) section.
### Easily Extensible and more control
* It's pretty easy to upgrade this code to your liking as it's a pretty simple and basic Sprite Animator.
* Unity's Animator don't give you enough control for 2D (and sometimes even for 3D) games, there isn't a easy way of doing things like: checking current frame, checking which animation you are, checking if animation has ended, etc...
  
## Benchmarking
For the benchmark i did a simple test on a empty URP project with 2D Rendering/Lighting and 10.000 2D characters playing a 5-frames-long idle animation, here's the results:
### Unity Default Animator
* Animator disabled: 130 FPS.
* Animator enabled: 15 FPS.
  
### Sprite Animator
* Animator disabled: 130 FPS.
* Animator enabled: 85 FPS.
  
## Where can i find further documentation about (codes and other things)?
That's as easy as [clicking here](DOCUMENTATION.md)
  
## How do i contribute to this project?
[Click here](CONTRIBUTING.md)
  
## Contact
**Discord**: *Gabriel Bigardi#2292*  
**Twitter**: *@BigardiGabriel*  
**Email**: *gabrielbigardi@hotmail.com*  
