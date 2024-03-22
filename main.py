import json
import random
from datetime import datetime, timedelta
import requests

def generate_stock_prices(start_price, num_days, trend_type, max_variation_percent):
    prices = []
    current_price = start_price
    for day in range(num_days):
        if trend_type == "wave":
            if day % 10 < 5:
                variation = random.uniform(0, max_variation_percent)
            else:
                variation = random.uniform(-max_variation_percent, 0)
        elif trend_type == "bull":
            variation = random.uniform(-max_variation_percent/2, max_variation_percent)
        else:  # bear
            variation = random.uniform(-max_variation_percent, max_variation_percent/2)

        current_price *= (1 + variation / 100)
        current_price = round(current_price, 2)
        prices.append(current_price)
    return prices

def generate_stock_json(symbol, name, start_date, num_days, initial_price, max_variation_percent, trend_type):
    data = {"symbol": symbol, "name": name, "prices": []}
    date = datetime.strptime(start_date, "%Y-%m-%d")
    prices = generate_stock_prices(initial_price, num_days, trend_type, max_variation_percent)
    for price in prices:
        data["prices"].append({"date": date.strftime("%Y-%m-%d"), "price": price})
        date += timedelta(days=1)
    return json.dumps(data, indent=4)

# Example usage
symbol = "AGASDAGG"
name = "AGGSADpple Inc."
start_date = "2024-01-31"
num_days = 160
initial_price = 150.00
max_variation_percent = 5.0
trend_type = "bull"  # Choose from "wave", "bull", "bear"

json_data = generate_stock_json(symbol, name, start_date, num_days, initial_price, max_variation_percent, trend_type)

# API call
url = 'http://localhost:8080/api/stock/addstock'
headers = {'Content-Type': 'application/json'}
# print(json_data)
response = requests.post(url, headers=headers, data=json_data)

print("Status Code:", response.status_code)
print("Response:", response.text)
