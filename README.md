# SimulationCSharpClient
The SimulationCSharpClient is a client for the 3DSIM simulation API. The model and client objects are auto generated using NSwag and the swagger 2.0 API Specification from https://github.com/3DSIM/simulation-api-specification

## Technical Specifications
### Platforms Supported
MacOS, Windows, and Linux

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

Currently using nswag version to generate client:
```
NSwag version: v11.12.7
NJsonSchema version: v9.10.6.0 (Newtonsoft.Json v9.0.0.0)
```

To install specific version
```
npm install -g nswag@11.12.7
```
## Creating swagger.json from swagger.yaml
Goto swagger.io and open the online editor. Paste the swagger.yaml from the https://github.com/3DSIM/simulation-api-specification project. In the file menu, select "Convert and Save as Json"

Reference this created json file in the next section

## Regenerating from swagger.json
To generate the client, run nswag in the project root folder using the swagger specification converted to json. You can convert the yaml file at editor.swagger.io by pasting the text into the editor window then downloading as json.

If converting from local file, copy json version of spec, `swagger.json` in this case, into root folder then run:
```
nswag swagger2csclient /input:swagger.json /classname:SimulationClient /namespace:SimulationCSharpClient.Client /output:src/SimulationCSharpClient/Client/SimulationClientGenerated.cs /generateclientinterfaces:true /injecthttpclient:true /ResponseArrayType:System.Collections.ObjectModel.ObservableCollection
```

## Contributing code
Read this article and follow the steps they outline: http://scottchacon.com/2011/08/31/github-flow.html

## Contributors
* Tim Sublette
* Ryan Walls
* Chad Queen
* Pete Krull
* Alex Drinkwater

## Original release
September 2017