### Todo (punch list)

Performance:
-Share a dictionary between styles and attributes; just set a bit flag and use an integer.
-Move indexing from CSSStyleDeclaration into DomElement (like attributes)
-Standarize the interface for CSSStyleDeclaration and AttributeCollection with the browser DOM

(done) nth-child could be alot more efficient by caching the index postions of all siblings when checking the 1st one.
(done) Cache equations AFTER parsing

