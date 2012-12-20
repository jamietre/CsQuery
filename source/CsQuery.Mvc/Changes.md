##CsQuery.MVC Framework Change Log

* Fixed a bug in regex causing dependencies to be unresolved if on a single line comments, e.g. `/* using something */`
* Optimized parsing during view rendering to only create a CQ object when the target is accessed
