Cornee de Ruiter, 1524623

My raytracer has all the minimum requirements. Aside from those, I have also implemented multi-threading. The application automatically decides how many cores to use for rendering.
All cores will be used, so on a 16 core system, 16 threads will be created for rendering.
All source code that is written by me (except for MyApplication.cs) will be in the folder: RayTracer within the root directory

When opening the application, some text will appear in the console that opens next to it. It first asks for a FOV. It must be above 0 and an integer. If those requirements
are not met, the same question pops up again. 
Once a FOV has been chosen, you can choose which debug rays you want visible. It first asks if you want to see the normal debug rays. They have a green color and are the most important.
Typing true will result in showing debug rays, anything else in not showing debug rays.
If you chose true for debug rays, it will also ask if you want to see shadow rays and/or secondary rays. Those have a blue and green color respectively.
Since the secondary and shadow rays will be drawn from the initial debug ray, sometimes you will see a ray drawn above another primitive. This is because the debug view is topdown, so height is not visible.
If this happens, a ray is under or above that primitive.

Once all the variables are set, the scene will be rendered. The camera positions at position 0,0,0, with a default lookat, 0,0,1. 
You can however move the camera. As usual with a keyboard, the wasd keys move the camera. W is go forward, S is go backwards, A is go left and D is go right.
To go up and down, you can press Z and X. 
To turn left or right, you can press N or M.
To turn upwards or downwards, you can press K or L.
To tilt left or right, you can press O or P.
For this to work, you have to have focus on the console. This means that you have to type the commands in the console but they will not appear on the console. Clicking on the scene will make it that you get a still image.
You can press the keys to move by 4 units each time, but you can also hold the key to go really fast. Watch out though, with high fps it moves really fast.
The scene will only rerender once the whole scene is done with rendering AND the user has moved the camera. Since multi-threading is enabled, continuously moving will not look pretty since rendering needs time.

If you want to add primitives to the scene, you can go to RayTracer/Raytracer.cs, line 53. There are instructions there to add primitives.

The sources used are the following:
https://raytracing.github.io/books/RayTracingInOneWeekend.html
https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
https://samsymons.com/blog/math-notes-ray-plane-intersection/
And of course the slides from the lectures.