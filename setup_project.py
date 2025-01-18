import os

def create_directory_structure():
    # Base path for Unity Assets folder
    base_path = "Assets"
    
    # Main directory structure
    directories = [
        "Scripts/Core",
        "Scripts/Game/Board",
        "Scripts/Game/Words",
        "Scripts/Game/Players",
        "Scripts/Game/PowerUps",
        "Scripts/Online",
        "Scripts/UI",
        "Scripts/Utils",
        "Scripts/Config",
        "Resources/Prefabs/Board",
        "Resources/Prefabs/UI",
        "Resources/Prefabs/Effects",
        "Resources/Materials",
        "Resources/Textures",
        "Resources/Fonts",
        "Resources/Audio",
        "StreamingAssets/dictionary"
    ]
    
    # Create directories
    for directory in directories:
        path = os.path.join(base_path, directory)
        os.makedirs(path, exist_ok=True)
        # Create .gitkeep to track empty directories
        gitkeep_path = os.path.join(path, ".gitkeep")
        with open(gitkeep_path, 'a'):
            pass
            
    print("Directory structure created successfully!")

if __name__ == "__main__":
    create_directory_structure()