(**
// can't yet format YamlFrontmatter (["title: Adding download badges"; "category: sample content"; "categoryindex: 1"; "index: 5"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

# Adding download badges

## Notebook download

`[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/4_download-badges.fsx)`

becomes

[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/4_download-badges.fsx)

(you might need to adjust the paths if there are any more levels between `http://localhost:8901/` and `img/badge-script.svg` or `4_download-badges`)

## Script download

`[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/4_download-badges.ipynb)`

becomes

[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/4_download-badges.ipynb)

(you might need to adjust the paths if there are any more levels between `http://localhost:8901/` and `img/badge-script.svg` or `4_download-badges`)

## Multiple badges in one line

To add multiple badges to appear on the same line like this:

[![Binder](http://localhost:8901/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=4_download-badges.ipynb)&emsp;
[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/4_download-badges.fsx)&emsp;
[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/4_download-badges.ipynb)

add a `&emsp;` after the first two badges in you markdown:

`[![Binder](http://localhost:8901/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=4_download-badges.ipynb)&emsp;`

`[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/4_download-badges.fsx)&emsp;`

`[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/4_download-badges.ipynb)`

*)

