# ensek
Ensek Test Repository

Bugs Identified:

(1) Bearer token used in reset endpoint after logging in but not for the other end points as they should all have the bearer token.

(2)  Confirm that purchasing energy units more than the total available updates quantity to zero - currenlty, can add negative values.

(3) "Oil" instead of "oil" in orders when adding a new oil entry as their is a default existing one with the variable as "oil" lower case.

(4) "Elec" instead of "electric" in orders when adding a new electric entry - existing default entry in orders has "electric". It has to conform with the Energy endpoint of "oil", "electric", "gas" and "nuclear".

(5) Energy type added as 'Bad Request' instead of 'Not Found' - If it can't find an energy type to add then it should be 'not found'

(6) Quantity shown as 'Not Found' when a negative value is added instead it should be a 'bad request'

(7) The purchased and untis remaining figure alarmed me on the PUT endpoint but they are the other way round. However, the energy quamtities are correctly reduced (until it goes negative).

(8) Not really a defect but still requires a number of retries in order to get the token - why internal server error?

(9) Total units of energy for all the energy types (except Nuclear) figure goes down to -1 when more than the allocated energy is used.


Code Base:

As the framework was put together relatively quicky, there are a few things I would have improved upon:

(1) Builder configuration in order to pass through the default Url, Access Token and login credentials - credentials likely to be hidden via Azure Key Vault (If Azure was used).

(2) Have the Acess Token avaialble for all the end points to use - as it is currently being used by the reset endpoint after logging in.

(3) Dealt with the non-nullable warnings for the DTO's after running a build.

(4) Added NLog where appropriate to help debug - useful to see how many retries have taken place on end points for a start. 

(5) Token generation (after login) is created under the [BeforeFeature] decorator.

(6) Reset endpoint is called under the [BeforeScenario] decorator for each scenario so that we have a clean slate.
