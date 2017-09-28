# SimulationCSharpClient
The SimulationCSharpClient is a client for the 3DSIM simulation API. The model and client objects are auto generated using NSwag and the swagger 2.0 API Specification

## Usage
See simulation-api-specification repo for constructor values:
https://github.com/3DSIM/simulation-api-specification

```
var simulationClient = new SimulationClient(Credentials.BaseURL, <Auth0TokenURL>, <Client ID>, <UserName>, <Password>, <connection>);

var simulations = this.simulationClient.AssumedStrainSimulationsGetAsync(2, null, null, 0, 10).Result;

foreach(var simulation in simulations)
{
	// do stuff with simulations
}
```

## Tools
nswag - https://github.com/RSuter/NSwag

nswag can be installed using npm

```
npm install nswag -g
```

## Regenerating from swagger.json
After installing nswag, run the following command from the project root:

```
nswag swagger2csclient /input:http://localhost:5000/swagger/v1/swagger.json /classname:SimulationClient /namespace:SimulationCSharpClient.Client /output:src/SimulationCSharpClient/Client/SimulationClientGenerated.cs
```

The QA or Production endpoint can be used of not running the API locally.

