// MODEL:

TestAssemblyEntity(
    TestType TestType,
    string Name,
    string Path)

TestAssembly(
    TestSuite[] RuntimeTests,
    TestSuite[] ApplicationTests)
    
TestSuite(
    TestCase[] TestCases,
    ...,
    string Path) 
    : TestAssemblyEntity(.., Path)
    
TestCase(...)
    : TestAssemblyEntity(...)

// API & LOGIC
INunitTestRunnerProxy
    RunTestAsync: 
    - Bude brát TestAssemblyEntity[]
    - Potřebuje znát pouze Path (podle toho se entita spustí )
    - Nepotřebuje Children (vše dostane na hromadě, jak testcasy tak testsuites)
    LoadAssemblyAsync: Navrátí TestAssemblyEntity