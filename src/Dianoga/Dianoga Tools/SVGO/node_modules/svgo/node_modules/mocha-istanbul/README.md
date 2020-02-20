mocha-istanbul
==============

[Mocha](http://visionmedia.github.com/mocha/) reporter to generate coverage report of [istanbul](http://gotwarlost.github.com/istanbul/) instrumented code.

Usage
-----

1. Install `istanbul`, `mocha` and `mocha-istanbul`
2. Instrument your code using `istanbul`
3. Make your tests to use the instrumented code (see [COA](https://github.com/veged/coa) library as an example)
4. Run `mocha` using `mocha-istanbul` as a reporter
5. Use the environment variable `ISTANBUL_REPORTERS` to specify a comma separated list of istanbul reports. By default `ISTANBUL_REPORTERS=text-summary,html`
