#### Step 1: Setup Python Environment

- Download the Python 3.8 from the [Official](https://www.python.org/downloads/release/python-380/) Site.
- Setup virtual environment through following command

    ```
    python --version 
    python -m venv .venv
    source .venv/bin/activate
    ```

Or run it in docker.

```bash
docker-compose up -d
docker-compose exec test_runner bash
```
#### Step 2: Download dependencies

```bash
pip install -r requirements.txt
```

In docker

```bash
docker-compose exec test_runner pip3 install --user -r requirements.txt
```

#### To experiment with the notebooks

Please install jupyter notebook in pip

```bash
pip install jupyter-notebook
```

Or in docker it is already installed.

Go to the notebook folder and execute jupyter notebook command. You will be redirected to the notebook session in
browser.

```bash
jupyter notebook
```

In docker

```bash
docker-compose exec test_runner bash
cd notebooks
jupyter notebook --ip 0.0.0.0
```

For Hello World, open notebooks folder in browser and run following file.

[Hello World Testing.ipynb](./notebooks/Hello World Testing.ipynb)

In order to do end to end model deployment, please follow the doc below.

[Model Deployment through notebook](./ModelDeployment.md)
