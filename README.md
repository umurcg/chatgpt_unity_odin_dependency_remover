# Unity Odin Inspector Dependency Remover

This project provides an automated solution for modifying Unity C# scripts to remove dependencies on Odin Inspector, making them suitable for open-source distribution. It utilizes OpenAI's GPT to refactor scripts, preserving functionality while removing proprietary dependencies.

## Features

- Automated script processing to remove Odin Inspector dependencies.
- Utilizes preprocessor directives for compatibility and maintains script functionality.
- Easy-to-use command-line interface for processing an entire directory of scripts.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- Python 3.10 or higher
- OpenAI API key (You can obtain it from [OpenAI](https://openai.com/))

## Installation

Clone the repository to your local machine:

```bash
git clone https://github.com/umurcg/chatgpt_unity_odin_dependency_remover.git
cd unity-odin-remover
```

## Usage

To use the script modification tool, follow these steps:

Place your .cs script files in the scripts directory.
Run the script with the command:
bash
Copy code
python modify_scripts.py 'path/to/your/directory'
Replace 'path/to/your/directory' with the path to the directory containing your Unity C# scripts.