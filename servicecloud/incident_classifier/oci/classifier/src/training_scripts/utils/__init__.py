import configparser
import os
from os.path import dirname, abspath

base_dir = dirname(dirname(dirname(abspath(__file__))))


def set_config():
    config = configparser.ConfigParser()
    config.read(os.path.join(base_dir, 'app.config'))

    for key, value in config.defaults().items():
        os.environ[key] = value
