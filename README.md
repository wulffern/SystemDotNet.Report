# SystemDotNet.Report 

Back in 2004 I was doing my Ph.D, and I kept running into a similar situation.

1. I have a file in a text format (.csv,.txt, something else)
2. I want to plot the data as a function of X/Y, and do some functions

At that time, I was working in a Windows 2000 environment, and there was not
really that many open solutions for plotting data. There was Excel, Matlab, and
probably something else.

During a summer job in 1999 I got hooked on programming, and during 2001 I spent
the summer working with the .NET framework for a "Lab-On-Web" application. 

As such, coding something in C# was easy, which ended up being NextGenLab.Chart
and SystemDotNet.Report [^1] 

Over the years, I've used the SystemDotNet.Report at infrequent times to see data.

In 2006 I switched to MacOS for all my private and hobby stuff, so it did not make sense to continue to develop the code.
I did not like the fact that I was working on a program that was locked to one platform (windows), so I stopped working on it.

Spool almost 20 years into the future, and it turns out that in 2022 it's now possible to use C# on Mac (and Linux).

Also, the original situation has again reared it's head, reading .raw simulation files from ngspice. 

The interface in ngspice is relatively basic, and it would be good to have a bit more flexible waveform viewer.

# Lower your expectations

The port is not likely to happen fast. But it's a fun project. See the issues list for the remaining tasks to get it running on a Mac.


[^1]: It's another story why it was called SystemDoNet. I was doing some work on ADCs and system modeling of ADCs. There was SystemC, but I did not like that, so I wrote SystemDotNet.
