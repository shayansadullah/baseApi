Feature: BuyFuel
As a User I should be able to add a quantity of each fuel type
with valid data
so that I can be sure that the 

Scenario Outline: Buy a quantity of each fuel type
	Given I add an order with <energyName> energy and <quantity> quantity
	Then the order has been created for <energyName> energy and <quantity> quantity

	Examples: 
	| energyName | quantity |
	| electric   | 4321     |
	| gas        | 2999     |
	| oil        | 19       |

Scenario: Confirm how many orders were created before the current date
	Given I add an order with gas energy and 200 quantity
    Then the number of orders created before the current date is 5
	
Scenario: Confirm adding an energy identifier that does not exist results in a Not Found status
	Given I add an order with an energy that does not exist
	Then 404 response code is returned

Scenario: Confirm adding an invalid quantity results in a Bad Request request
	Given I add an order with an energy quantity that is negative
	Then 400 response code is returned

Scenario: Confirm that the energy calculations are as expected when purchasing Gas
	Given I add an order with gas energy and 100 quantity
	Then the purchase of 100 quantity at 34.0 cost with 2900 units are remaining

Scenario: Confirm that the energy quantity is affected correctly based on the quantity purchased for Gas
	Given I add an order with gas energy and 1 quantity
	Then the energy quantity of units for gas should be 2999

Scenario: Confirm that the energy calculations are as expected when purchasing Electric
	Given I add an order with electric energy and 100 quantity
	Then the purchase of 100 quantity at 47.0 cost with 4222 units are remaining

Scenario: Confirm that the energy calculations are as expected when purchasing Oil
	Given I add an order with oil energy and 5 quantity
	Then the purchase of 5 quantity at 3.0 cost with 15 units are remaining

Scenario Outline: Confirm that usurping all the energy results in no more fuel to purchase 
	Given I add an order with <energyName> energy and <quantity> quantity
	Then the message should appear that there is no <energyName> fuel to purchase

	Examples: 
	| energyName | quantity |
	| nuclear    | 10       |
	| electric   | 4323     |
	| gas        | 3001     |
	| oil        | 21       |
	
Scenario Outline: Confirm that purchasing energy units more than the total available updates quantity to zero
	Given I add an order with <energyName> energy and <quantity> quantity
	Then the energy quantity of units for <energyName> should be 0

	Examples: 
	| energyName | quantity |
	| gas        | 3001     |
	| nuclear    | 10       |
	| electric   | 4323     |
	| oil        | 21       |

	