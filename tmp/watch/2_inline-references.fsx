(**
// can't yet format YamlFrontmatter (["title: Inline package references and charting"; "category: sample content"; "categoryindex: 1"; "index: 3"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

[![Binder](http://localhost:8901/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=2_inline-references.ipynb)&emsp;
[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/2_inline-references.fsx)&emsp;
[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/2_inline-references.ipynb)

[How to add these badges?](http://localhost:8901/4_download-badges.html)

# Inline package references and charting

With fsdocs 8.0, the tool can roll forward to .net 5, meaning you can use inline package references in the docs scripts:
*)
#r "nuget: Plotly.NET, 2.0.0-preview.6"

open Plotly.NET

let myChart = 
    Chart.Line(
        [
            1.,1.
            5.,6.
            23.,9.
        ]
    )
    |> Chart.withTitle "Hello fsdocs!"
(**
You can now also include raw html in your docs scripts with the new `include-it-raw`.
To incude the chart html of a Plotly.NET chart and and render it on the docs page, use the `GenericChart.toChartHTML`
and include the raw output. 

the actual codeblock looks like this:

<pre>
(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)
</pre>
</pre>

Here is the rendered chart:
<div id="f1b6f943-9b3c-412e-81e0-603cad92890a" style="width: 600px; height: 600px;"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_f1b6f9439b3c412e81e0603cad92890a = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-latest.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","x":[1.0,5.0,23.0],"y":[1.0,6.0,9.0],"mode":"lines","line":{},"marker":{}}];
            var layout = {"title":"Hello fsdocs!"};
            var config = {};
            Plotly.newPlot('f1b6f943-9b3c-412e-81e0-603cad92890a', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_f1b6f9439b3c412e81e0603cad92890a();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_f1b6f9439b3c412e81e0603cad92890a();
            }
</script>

*)

