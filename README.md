# RestaurantErp Application Unit Testing (Optional)

This repository contains unit tests for the RestaurantErp.

## Table of Contents
- [Introduction](#introduction)
- [Prerequisites](#prerequisites)
- [Unit Testing](#unit-testing)
- [How to Run Tests](#how-to-run-tests)

## Introduction

The RestaurantErp application is an Enterprise Resource Planning (ERP) solution for restaurants. It handles order creation, editing, and checkout operations. The main class for this application is `OrderProvider`, which is responsible for managing orders and calculating bills.

## Prerequisites

To run the unit tests, you will need the following:

- Visual Studio or an IDE of your choice that supports NUnit testing.
- The RestaurantErp application solution.

## Unit Testing

We will be writing unit tests using NUnit for the following public methods of the `OrderProvider` class:

1. `CreateOrder()`: This method creates a new order and returns its unique identifier (orderId). We will test the order creation process and verify that each order has a unique identifier.

2. `AddItem()`: This method adds information about a new item to an existing order based on the provided `OrderItemRequest`. We will test the process of adding items to an order, including verifying that the correct item details are stored.

3. `CancelItem()`: This method removes information about an already added item from an existing order based on the provided `OrderItemRequest`. We will test the cancellation process to ensure that the correct item is removed from the order.

4. `Checkout()`: This method calculates the bill for a given order and returns a `BillExternal` entity with various fields. We will test the checkout process, including checking the correctness of bill calculations, discounts, service charges, and the final bill.

We will also test scenarios related to:
- Logic of discounting by time.
- Logic of service charge (if it is 0%, 10%, etc.).
- Feature integrations, such as adding and removing items with various conditions.

## How to Run Tests

1. Clone this repository to your local machine.

2. Open the solution in your preferred IDE (e.g., Visual Studio).

3. Build the solution.

4. Run the NUnit tests for the `OrderProvider` class, focusing on the `CreateOrder()`, `AddItem()`, `CancelItem()`, and `Checkout()` methods.

5. Analyze the test results to ensure that all test cases pass successfully.

