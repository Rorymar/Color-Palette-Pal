# The Color Palette Pal
A downloadable, offline application that aims to simplify the creation and accessibility of color palettes for artists, designers, and hobbyists alike.

### Acknowledgements
This tool was developed by the Color Palette Pals—Kiron Das, Salsa Essader, Lee Farmer, Rory Martin, and Aidan Rosenblatt—as part of CSCI 4440: Software Design and Documentation at Rensselaer Polytechnic Institute (RPI). Created over the course of four months, the project was accompanied by comprehensive documentation submitted to fulfill the course requirements. You can find our GitHub repository here: https://github.com/Rorymar/Color-Palette-Pal.

### Features
Tools to Generate Palettes
- Palette from Scratch; Generate random palettes, customize colors manually, and save your designs.
- Palette from Image; Extract a palette from the colors in an image the user uploads.
Tools to Check Accessibility
- Color Blindness Toggles; View how palettes look under different types of color blindness.
- Image Filter for Color Blindness; View how an image looks under different types of color blindness.
- Color Blindness Palette Adjustor; Changes your palette to be distinguishable under color blindness.

# Installation Instructions
TBA

# Coding Standards
## Programming Practices
### Commenting
Comments should be concise, relevant, and informative. Provide comments before functions, methods, classes, and other significant code segments, describing their purpose and functionality.
### Naming Conventions
Use consistent and descriptive variable names. Avoid overly lengthy names or those that conflict with other code elements. Follow camelCase for variables and PascalCase for functions and classes.
### Spacing
Use a tab width of 4 spaces. Include one blank line between function definitions for clarity.
### Brace Placement
Place opening braces on the line immediately following class declarations, function declarations, and loop conditions.

## Unity Practices  
### Scenes  
Create a new scene for each major feature (e.g., Palette From Scratch). Each scene must include the following:  
- A back button with a white arrow in the bottom-left corner, navigating to an existing page.  
- Italicized white text at the top displaying the feature title.  
- The image `Assets/Images/thepal.png` in the top-right corner.  
- The text "Made by the Color Palette Pals" in the bottom-right corner.  
Scenes should also feature two bars with the color `#193456` at the top and bottom of the page, where the specified elements will be placed. All scenes must be stored in the `Assets/Scenes/DevScenes` folder.  
### UI Elements  
All UI elements must be placed on a main canvas. Group similar UI elements into empty GameObjects for better organization when there are three or more similar elements (e.g., `Color1` in `PaletteManager`). Grouping is also encouraged for fewer elements if deemed necessary for clarity.  
Clickable buttons (excluding the back button) must use the color `#477FAE`. All UI elements should contrast adequately with the backgrounds they are placed on.  
### Images  
Images must be stored in the `Assets/Images` folder and be of high quality. Ensure images have high contrast.  
### Scripts  
Scripts must be stored in the `Assets/Scripts` folder.  
### Materials  
Materials must be stored in the `Assets/Materials` folder.  

## Other General Project Practices  
### GitHub Model  
The project will include a project branch and a developer branch. Only stable, tested code should be merged into the project branch. If needed, create feature branches from the developer branch.  
### Commits
Commit regularly and include detailed comments on the changes. Commits should not contain unfinished work or unstable features unless documented with a high-priority issue on Jira and communicated to all team members.
