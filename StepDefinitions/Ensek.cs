using Ensek.Helpers;
using Ensek.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Text.RegularExpressions;
using Ensek.Enums;
using System.Net;

namespace Ensek.StepDefinitions
{
    [Binding]

    public class Ensek
    {
        private ScenarioContext _scenarioContext;

        const int energyIdDoesNotExist = 20;

        public Ensek( ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [StepDefinition(@"I add an order with (.*) energy and (.*) quantity")]
        public async Task GivenIAddAnOrderWithAnd(Energy energyName, int quantity)
        {
            int id = (int)energyName;
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/buy/{id}/{quantity}", Method.PUT);
            var pattern = @"([a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})";
            var test = response.Content; 
            var match = Regex.Match(test, pattern);
            if (match.Success)
            {
                var matchedItem = match.Groups[1].Value;
                _scenarioContext["orderDetailsId"] = matchedItem;   
            }
            _scenarioContext["orderResponse"] = response?.Content; 
        }

        [StepDefinition(@"the number of orders created before the current date is (.*)")]
        public async Task ThenTheNumberOfOrdersCreatedBeforeTheCurrentDateIs(int countExpectedOrders)
        {
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/orders", Method.GET);
            var listOfOrders = JsonConvert.DeserializeObject<List<Order>>(response.Content);
            var today = DateTime.Today;
            var countOrdersBeforeCurrentDate = listOfOrders?.Count(o => o.time < today) ?? 0;
            countOrdersBeforeCurrentDate.Should().Be(countExpectedOrders);
        }

        [StepDefinition(@"the order has been created for (.*) energy and (.*) quantity")]
        public async Task ThenTheOrderHasBeenCreated(Enums.Energy energyName, int quantity)
        {
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/orders", Method.GET);
            var listOfOrders = JsonConvert.DeserializeObject<List<Order>>(response.Content);
            var order = listOfOrders?.FirstOrDefault(order => order.id == _scenarioContext["orderDetailsId"].ToString());

            order?.id.Should().Be(_scenarioContext["orderDetailsId"].ToString(), 
                    $"Order has not been created: {order.id}");
            
            order?.quantity.Should().Be(quantity, $"Order quantity of {quantity} is not present!");
            
            order?.fuel.Should().Be(energyName.ToString(), $"Energy name is not correct {energyName.ToString()}");
        }

        [StepDefinition(@"I add an order with an energy that does not exist")]
        public async Task GivenIAddAnOrderWithAnEnergyThatDoesNotExist()
        {
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/buy/{energyIdDoesNotExist}/1", Method.PUT);
            _scenarioContext["statusCode"] = response?.StatusCode;
        }

        [StepDefinition(@"I add an order with an energy quantity that is negative")]
        public async Task GivenIAddAnOrderWithAnEnergyQuantityThatIsNegative()
        {
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/buy/3/-1", Method.PUT);
            _scenarioContext["statusCode"] = response?.StatusCode;
        }

        [StepDefinition(@"(.*) response code is returned")]
        public void ThenResponseCodeIsReturned(HttpStatusCode statusCode)
        {
            _scenarioContext["statusCode"].Should().Be(statusCode, $"Expected {statusCode} but got: {_scenarioContext["statusCode"]}");
        }

        [StepDefinition(@"the energy quantity of units for (.*) should be (.*)")]
        public async Task ThenTheEnergyGasQuantityOfUnitsShouldBe(Energy energyType, int quantity)
        {
            var response = await RestSharApiHelper.SendRequest($"{Constants.RootUrl}/energy", Method.GET);
            var listOfEnergySources = JsonConvert.DeserializeObject<EnergyDto>(response.Content);

            switch(energyType)
            {
                case Energy.gas:
                    listOfEnergySources?.gas.quantity_of_units.Should().Be(quantity, $"Energy quantity is: " +
                        $"{listOfEnergySources?.gas.quantity_of_units} " +
                        $"but expected: {quantity}");
                    break;
                case Energy.nuclear:
                    listOfEnergySources?.nuclear.quantity_of_units.Should().Be(quantity, $"Energy quantity is: " +
                        $"{listOfEnergySources?.nuclear.quantity_of_units} " +
                        $"but expected: {quantity}");
                    break;
                case Energy.electric:
                    listOfEnergySources?.electric.quantity_of_units.Should().Be(quantity, $"Energy quantity is: " +
                        $"{listOfEnergySources?.electric.quantity_of_units} " +
                        $"but expected: {quantity}");
                    break;
                default:
                    listOfEnergySources?.oil.quantity_of_units.Should().Be(quantity, $"Energy quantity is: " +
                        $"{listOfEnergySources?.oil.quantity_of_units} " +
                        $"but expected: {quantity}");
                    break;

            }

        }

        [Then(@"the purchase of (.*) quantity at (.*) cost with (.*) units are remaining")]
        public void ThenThePurchaseOfQuantityAtCostWithUnitsAreRemaining(int quantity, double cost, int unitsRemaining)
        {
            var purchasedPattern = @"(purchased (\d+))";
            var testPurchasePattern = _scenarioContext["orderResponse"].ToString();
            var matchPurchasePattern = Regex.Match(testPurchasePattern, purchasedPattern);

            if (matchPurchasePattern.Success)
            {
                var matchedItem = matchPurchasePattern.Groups[1].Value;
                matchedItem.Should().Contain(quantity.ToString());
            }

            var costPattern = @"(cost of (\d+.\d+))";
            var testCostOfPattern = _scenarioContext["orderResponse"].ToString();
            var matchCostOfPattern = Regex.Match(testCostOfPattern, costPattern);

            if (matchCostOfPattern.Success)
            {
                var matchedItem = matchCostOfPattern.Groups[1].Value;
                matchedItem.Should().Contain(cost.ToString());
            }
            
            var unitsRemainingPattern = @"(there are (\d+.\d+) units remaining)";
            var testUnitsRemainingPattern = _scenarioContext["orderResponse"].ToString();
            var matchUnitsRemainingPattern = Regex.Match(testUnitsRemainingPattern, unitsRemainingPattern);

            if (matchUnitsRemainingPattern.Success)
            {
                var matchedItem = matchUnitsRemainingPattern.Groups[1].Value;
                matchedItem.Should().Contain(unitsRemaining.ToString());
            }
        }

        [Then(@"the message should appear that there is no (.*) fuel to purchase")]
        public void ThenTheMessageShouldAppearThatThereIsNoFuelToPurchase(Energy energyName)
        {
            var expectedText = $"There is no {energyName} fuel to purchase";
            var actualText = _scenarioContext["orderResponse"].ToString();
            actualText.Should().Contain(expectedText, $"Text does not contain: {expectedText}");
        }
    }
}
