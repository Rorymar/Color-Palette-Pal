# Color-Palette-Pal
A color palette creator meant to help artists and web designers create both attractive and accessible palettes

# Programming Practices
## Commenting
A comment should be concise, relevant, and informative. Always comment before a function, method, class, etc. about the role of said code.
## Naming Conventions
Variable names should be consistent and understandable. Avoid overly long variable names and names matching other elements of the code. For style consistency, variable names should be camel-case, plus function and class names should be pascal-case. 
## Spacing
Tab spacing should be 4 characters wide and there should be one empty line between function definitions.
## Brace Placement
Braces should begin on the line following that of the class declaration, function declaration, and loop conditions.

# Unity Practices
## Scenes
New scenes should be created for every new major feature (ie, Palette From Scratch). In addition, every scene must have a back button with a white arrow in the bottom left corner to go to an existing navigation page plus italicized white text at the
top with the feature title. Every scene must also include Assets/Images/thepal.png in the top right corner and "made by the color palette pals" in the bottom right corner. Also, scenes should have two bars of the color #193456 on the top and bottom of the page, where the elements previously defined will be located. Scenes should be stored in the Assets/Scenes/DevScenes folder.
## UI elements
All UI elements need to be placed on a main canvas. Also, UI elements should be contained within empty game objects for organization purposes when there are three or more similar objects (ie, Color1 in the PaletteManager). This is welcome for fewer objects if the developer deems the extra game object important for the organization.
Clickable buttons excluding the back button must be the color #477FAE, and all other UI elements must contrast with the UI elements they are placed on top of. 
## Images
Images should be placed in the Assets/Images folder and should be high quality. Images used should have high contrast.
## Scripts
Scripts should be placed in the Assets/Scripts folder. 
## Materials
Materials should be placed in the Assets/Materials folder.

# Project Practices
## Github Model
The project will have a project branch and a developer branch. Only stable and tested code should be applied to the project branch. If necessary, create feature branches of the dev branch.
## Commits
Commit often, and always comment on the changes in your commits. Commits should not contain any unfinished work or program-breaking features that are not documented with a high-priority issue on Jira and are known by all team members.
