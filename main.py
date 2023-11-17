import os
import openai

def fix_script(script_file):
    # Read the example scripts
    with open(os.path.join(".", "unmodified_example.cs"), "r") as file:
        original_script = file.read()
    with open(os.path.join(".", "modified_example.cs"), "r") as file:
        modified_script = file.read()

  
    script_to_be_modified = script_file.read()

            # Prepare the prompt for ChatGPT
    prompt = [
        {
            "role": "system",
            "content": "You are a highly skilled AI assistant experienced in C# programming and familiar with Unity and Odin Inspector. Your task is to help modify Unity C# scripts, removing Odin Inspector dependencies while maintaining functionality. Use directives to conditionally include Odin-specific code. You don't have to replicate functionality of Odin Inspector. There should be only code without any explanation or comments.",
        },
        {
            "role": "user",
            "content": f"Here is an example of a script before and after I manually removed Odin Inspector dependencies: Original Script: csharp\n{original_script}\nModified Script: csharp\n{modified_script}\nAs you can see, in the modified script, I have used preprocessor directives to conditionally include Odin Inspector features. Where Odin Inspector attributes were used, I have either removed them or replaced them with equivalent standard Unity attributes or custom implementations. I have also added or modified certain elements to ensure functionality remains intact without Odin Inspector.",
        },
        {
            "role": "system",
            "content": "Understood. Please provide the script you want to modify.",
        },
        {
            "role": "user",
            "content": f"Script to be Modified: csharp\n{script_to_be_modified}",
        },
    ]

    response = client.create(model="gpt-4-1106-preview", messages=prompt)

    modified_content = response.choices[0].message.content
    #Clear ```csharp from the beginning and ``` from the end
    modified_content = modified_content.replace("```csharp", "")
    modified_content = modified_content.replace("```", "")

    return modified_content
    


def modify_scripts(directory, openai_api_key):
    # Set the OpenAI API key
    openai.api_key = openai_api_key
    client = openai.ChatCompletion()
    
    

    # Traverse the directory
    for root, dirs, files in os.walk(directory):
        for file in files:
            # Print file name
            if file.endswith(".cs"):  # assuming the scripts are C# files
                file_path = os.path.join(root, file)
                with open(file_path, "r") as script_file:
                    
                    #if there is not Odin Inspector in the script, skip it
                    if "OdinInspector" not in script_file.read():
                        continue

                    # Fix the script
                    modified=fix_script(script_file)
                
                    # Write the modified script to the file
                    with open(file_path, "w") as script_file:
                        script_file.write(modified)
        


# Usage
# Get api key from api_key.txt
api_key = open("api_key.txt", "r").read()
openai.api_key = api_key
client = openai.ChatCompletion()
client.api_key = api_key


with open(os.path.join(".", "sample.cs"), "r") as file:
    fix_script(file)

# directory_to_scan = "../Packages/com.reboot.core/Scripts"

# modify_scripts(directory_to_scan, openai_api_key)
