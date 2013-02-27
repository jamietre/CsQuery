$(document).ready(function () {
    $('script[type=syntaxhighlighter]').attr("class", "brush:csharp");

    SyntaxHighlighter.defaults['toolbar'] = false;
    SyntaxHighlighter.defaults['gutter'] = false;
    SyntaxHighlighter.all();
});
Cufon.replace('h1,h2,h3,h4,.nav-bar > li a'); // Works without a selector engine