################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:54 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: dbb5811631bab09ee5e87c67f6067b299e3ccd78 $
################################################################################################
#  File: run_notebook.py
################################################################################################
import os
import logging
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

LOGGING_LEVEL = logging.INFO
LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'
logging.basicConfig(filename=F"{__file__}.log", level=LOGGING_LEVEL, format=LOGGING_FORMAT)

# initializing webdriver for Chrome with our options
CHROME_DRIVER_PATH = './chromedriver'
if os.path.exists(CHROME_DRIVER_PATH):
    # instance of Options class allows
    # us to configure Headless Chrome
    options = Options()

    # this parameter tells Chrome that
    # it should be run without UI (Headless)
    options.headless = True
    options.add_argument("--disable-dev-shm-usage")  # overcome limited resource problems
    options.add_argument("--no-sandbox")  # Bypass OS security model
    options.add_argument("--remote-debugging-port=9222")

    options.add_argument("disable-infobars")
    options.add_argument("--disable-extensions")
    driver = webdriver.Chrome(options=options, executable_path=CHROME_DRIVER_PATH)
    logging.info("*** Headless Chrome Session Started ***")
else:
    driver = webdriver.Chrome()
    logging.info("*** GUI Chrome Session Started ***")

wait = WebDriverWait(driver, 10)
driver.get(os.getenv("NOTEBOOK_SESSION_URL"))
driver.implicitly_wait(time_to_wait=10)
logging.info("*** Notebook URL Connected ***")


def bypoass_coockies_popup():
    time.sleep(5)
    isPopupVisible = len(driver.find_elements(By.CLASS_NAME, "truste_popframe"))
    if isPopupVisible:
        driver.switch_to.frame(driver.find_element(By.CLASS_NAME, "truste_popframe").get_dom_attribute("id"))
        driver.find_element(By.XPATH, '//a[text()="Accept all"]').click()
        driver.switch_to.parent_frame()


def is_conda_folder_exists():
    time.sleep(2)
    isCondaFolderExists = len(driver.find_elements(By.XPATH, "//span[text()='conda']"))
    if not isCondaFolderExists:
        terminal_input.send_keys("rm -rf ./conda/")
        logging.info("*** Previous Conda Removed ***")
        time.sleep(2)
        terminal_input.send_keys(Keys.ENTER)


def wait_until_component_is_visible(by, path, counts=None):
    if counts is None:
        counts = []
    isVisible = driver.find_elements(by, path)
    if isVisible:
        return
    else:
        if len(counts) >= 20:
            raise Exception(f"Element on path {path} not found")
        logging.info(f"***  Wait for {path} to get visible count {len(counts)}***")
        time.sleep(120)
        return wait_until_component_is_visible(by, path, counts=counts)


logging.info("***  Login Into the Notebook ***")
bypoass_coockies_popup()
driver.find_element(By.ID, "tenant").send_keys(os.getenv("TENANCY"))
wait.until(EC.visibility_of_element_located((By.ID, "submit-tenant")))
driver.find_element(By.ID, "submit-tenant").click()
wait.until(EC.visibility_of_element_located((By.ID, "direct-login-form-toggle")))
driver.find_element(By.ID, "direct-login-form-toggle").click()
confirm_pass = os.getenv("SELENIUM_CONFIRM_PASS")
driver.find_element(By.ID, "username").send_keys(os.getenv("SELENIUM_USER"))
driver.find_element(By.ID, "password").send_keys(os.getenv("SELENIUM_PASSWORD"))
driver.find_element(By.ID, "submit-native").click()
driver.implicitly_wait(time_to_wait=60)
logging.info("***  Login Success ***")

# Change the password
time.sleep(2)
bypoass_coockies_popup()
wait.until(EC.visibility_of_element_located((By.ID, "currentPassword")))
driver.find_element(By.ID, "currentPassword").send_keys(os.getenv("SELENIUM_PASSWORD"))
driver.find_element(By.ID, "newPassword").send_keys(confirm_pass)
driver.find_element(By.ID, "confirmPassword").send_keys(confirm_pass)
driver.find_element(By.NAME, "submit").click()
time.sleep(10)

# Open Notebook Terminal
logging.info("*** Open Notebook Terminal ***")
wait.until(EC.visibility_of_element_located((By.ID, "jp-top-panel")))
top_panel = driver.find_element(By.ID, "jp-top-panel")
file_menu = top_panel.find_element(By.XPATH, "//div[text()='File']")
time.sleep(5)
file_menu.click()
wait.until(EC.visibility_of_element_located((By.XPATH, "//div[text()='New']")))
new_menu = top_panel.find_element(By.XPATH, "//div[text()='New']")
time.sleep(2)
new_menu.click()
driver.implicitly_wait(5)
terminal_menu = top_panel \
    .find_element(By.XPATH, "//div[contains(@class, 'lm-Menu-itemLabel p-Menu-itemLabel') and text()='Terminal']")
time.sleep(1)
terminal_menu.click()
time.sleep(5)
logging.info("*** Got Terminal Window ***")

wait.until(EC.visibility_of_element_located((By.CLASS_NAME, "xterm-cursor-layer")))
terminal_input = driver.find_element(By.CLASS_NAME, "xterm-cursor-layer").parent.switch_to.active_element

is_conda_folder_exists()
time.sleep(2)
RAW_GIT_ENV_URL = os.getenv("RAW_GIT_ENV_URL")
terminal_input.send_keys(f"wget {RAW_GIT_ENV_URL}")
time.sleep(2)
terminal_input = driver.find_element(By.CLASS_NAME, "xterm-cursor-layer").parent.switch_to.active_element
terminal_input.send_keys(Keys.ENTER)
time.sleep(2)
logging.info("*** Creating New Conda Package ***")
terminal_input.send_keys("odsc conda create --file ./environment.yml --name b2c_env --slug b2c_env_slug "
                         "&& touch /home/datascience/conda_created")
logging.info("*** New Conda Package Created ***")
time.sleep(2)
terminal_input.send_keys(Keys.ENTER)
terminal_input.send_keys(Keys.ENTER)
wait_until_component_is_visible(By.XPATH, "//span[text()='conda_created']")
time.sleep(2)
terminal_input.send_keys(Keys.CONTROL + 'c')

BUCKET_NAME = os.getenv("BUCKET_NAME")
NAMESPACE = os.getenv("NAMESPACE")
terminal_input.send_keys(f"odsc conda init -b {BUCKET_NAME} -n {NAMESPACE} --authentication resource_principal")
time.sleep(2)
terminal_input.send_keys(Keys.ENTER)
time.sleep(2)
terminal_input.send_keys("odsc conda publish -s b2c_env_slug && touch /home/datascience/conda_published")
terminal_input.send_keys(Keys.ENTER)
terminal_input.send_keys(Keys.ENTER)
wait_until_component_is_visible(By.XPATH, "//span[text()='conda_published']")
time.sleep(2)
terminal_input.send_keys(Keys.CONTROL + 'c')
time.sleep(5)
terminal_input.send_keys(Keys.CONTROL + 'd')
time.sleep(10)
driver.close()
logging.info("Successfully published conda package")
