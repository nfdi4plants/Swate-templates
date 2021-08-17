(**
// can't yet format YamlFrontmatter (["title: Including notebooks"; "category: sample content"; "categoryindex: 1"; "index: 4"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

[![Binder](http://localhost:8901/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=3_notebooks.ipynb)&emsp;
[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/3_notebooks.fsx)&emsp;
[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/3_notebooks.ipynb)

[How to add these badges?](http://localhost:8901/4_download-badges.html)

# Including notebooks 

To include dotnet interactive notebooks in the ipynb format, it is enough for the `_template.ipynb` file to simply exist.

There are however some customization options with fsdocs that move your documentation to the next level:

## Conditional package references

use the IPYNB compiler directive in conjuntion with `condition:ipynb` to include blocks only in the rendered notebook file. 

This is especially usefull for referencing packages that otherwise be referenced locally during yopur buildchain:

<pre>
#r "/path/to/your/binaries/during/local/build"

(***condition:ipynb***)
#if IPYNB
#r "nuget: yourProjectOnNuget, 1.3.3.7"
#endif // IPYNB
</pre>

## Conditional value inclusion

Sometimes the content you want to include might differ aswell. An example is Plotly.NET charts. 
While you want to dump the chart html directly into the html docs via (`include-it-raw`), 
you want to end cells in notebooks with the chart value itself to include the chart in the output cell with Plotly.NET.Interactive. 

Here is an example for such an conditional block:

<pre>
open Plotly.NET

let myChart = Chart.Point([1.,2.])

(***condition:ipynb***)
#if IPYNB
myChart
#endif // IPYNB

(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)
</pre>

*)
open Plotly.NET

let myChart = Chart.Point([1.,2.])(* output: 
<div id="57596ada-245e-443f-a28a-b991e3ab16cb" style="width: 600px; height: 600px;"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_57596ada245e443fa28ab991e3ab16cb = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","x":[1.0],"y":[2.0],"mode":"markers","marker":{}}];
            var layout = {};
            var config = {};
            Plotly.newPlot('57596ada-245e-443f-a28a-b991e3ab16cb', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_57596ada245e443fa28ab991e3ab16cb();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_57596ada245e443fa28ab991e3ab16cb();
            }
</script>
*)
(**
## Including binder links

[Binder]() is an awesome project that launches an instance of your notebook given the correct `Dockerfile` and `nuget.config`, which will be added automatically by the `fsdocs` tool when you build the docs.

you can include a binder link like this (supposed you use gh-pages to host your docs):

```
[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/<YOUR-ORG>/<YOUR-PROJECT>/gh-pages?filepath=<YOUR-DOCS-FILENAME>.ipynb)
```

In fact, you can use this link here to check the conditionals of this very page in a notebook:

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/fslaborg/docs-template/gh-pages?filepath=3_notebooks.ipynb.ipynb)

*)

