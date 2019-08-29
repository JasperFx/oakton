<!--title:Environment Checks-->

The big out of the box feature with Oakton.AspNetCore is the ability to expose environment checks or environment tests directly
in your application. Most applications don't live in isolation. Your code probably has to interact with databases, the file system, configuration mechanisms, and other services. And since it's an imperfect world, it's quite possible that you've made a deployment in your career and later found out that configuration items were wrong, your dependencies were down, the database credentials you were using were wrong, or all kinds of sundry run of the mill problems.

Years ago I worked on a system that had worlds of environmental issues during deployments, and after learning about the technique of "environment tests" where you build in automatic checks in your deployment to exercise your system's integrations, we adapted the approach described here: [Environment Tests and Self-Diagnosing Configuration](http://codebetter.com/jeremymiller/2006/04/06/environment-tests-and-self-diagnosing-configuration-with-structuremap/).

Flash forward to 2019 (at the time of this release), and Oakton.AspNetCore allows you to build environment checks right into your ASP.Net Core application such that you can verify successful deployments to verify the system configuration quickly rather than waiting for user error reports or testers being blocked because the system is down (which happened to me today as a matter of fact).



## Adding Environment Checks


## Running Environment Checks


```
dotnet run -- check-env
```

```
Running Environment Checks
   1.) Success: good
   2.) Success: also good
All environment checks are good!
```


```
dotnet run -- ? check-env
```


```
 Usages for 'check-env' (Execute all environment checks against the application)
  check-env [-f, --file <file>] [-e, --environment <environment>] [-v, --verbose] [-l, --log-level <logleve>] [----config:<prop> <value>]

  ------------------------------------------------------------------------------------------------------------------
    Flags
  ------------------------------------------------------------------------------------------------------------------
                  [-f, --file <file>] -> Use to optionally write the results of the environment checks to a file
    [-e, --environment <environment>] -> Use to override the ASP.Net Environment name
                      [-v, --verbose] -> Write out much more information at startup and enables console logging
          [-l, --log-level <logleve>] -> Override the log level
          [----config:<prop> <value>] -> Overwrite individual configuration items
  ------------------------------------------------------------------------------------------------------------------
```



## Custom Environment Checks



## Check Factory


## Required Files



## Required Service Registration



## Required Configuration Value