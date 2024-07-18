Crown copyright (c), 2021-2024

## SAPIENT BSI Flex 335 v2 Test Harness
This software is a tool for developers of [SAPIENT](https://www.gov.uk/guidance/sapient-autonomous-sensor-system) 
components to use to test compliance of their component to the SAPIENT standard - [BSI Flex 335](https://knowledge.bsigroup.com/products/bsi-flex-335-v2-0-2023-sapient-network-of-autonomous-sensors-and-effectors-interface-control-document-specification-specification?version=standard).

## Dependencies
This software requires [PostgreSQL](https://www.postgresql.org/) (version 12) to be installed prior 
to installation of the test harness software. Later versions of PostgreSQL do not work with the
test harness software.

## System requirements
The SAPIENT Test Harness software runs on the Microsoft Windows operating system. Two system 
configurations have been successfully demonstrated. Other configurations are untested and no
guarantee can be made as to whether they will work or not.

| Dependency | Version | Version |
|------------|---------|---------|
| Microsoft Windows | Windows 10 Business Edition (x64), 22h2 | Windows 11 Home (x64), 23H2 |
| Microsoft .NET 6 SDK | 6.0.408 | 6.0.421 |
| PostgreSQL (x64) | 12.14.2 | 12.18.1 |
| Microsoft Visual Studio | Visual Studio Professional 2022 (x64), LTSC 17.6.2 | Visual Studio Community 2022 (x64), 17.9.6 |

Note: Microsoft Visual Studio is only required if you intend to compile the software from the 
source code.

The minimum hardware requirements for running the SAPIENT test harness are listed below:

| System Element | Specification |
|----------------|---------------|
| Processor | 4 core, 2.60 GHz, 8 Mb processor cache |
| Memory | 16 Gb |
| Hard Drive | 512 Gb |

## Build
Consult the User Guide for instructions to build the software from the source code.

## Configuring and Running the Test Harness
Consult the User Guide for full instructions for configuring and running the software. It should
be noted that configuration of the Test Harness is split across multiple config files which a 
novice user may find confusing. It is recommended to re-install and re-configure the test harness
from a clean copy of the software should any difficulties be experienced, particularly with 
network connections of components being tested.

## Limitations
This version (5.2.3) of the SAPIENT Test Harness is only compatibile with the BSI Flex 335 version 2 
of SAPIENT. The test harness is not capable of testing earlier versions of SAPIENT. Although SAPIENT
components can be developed/run on a platform of the developers choice, the test harness software
only runs on Microsoft Windows platforms.

## Known Issues
1.) It has been noted that running the Test Harness software and PGadmin simultaneously may cause
CPU to run at, or near to 100%. This may cause latency and other poor performance issues, but 
should not impact functionality. It is recommended not to run these two applications at the same 
time.

2.) The timestamp in PGadmin does not align with the test harness timestamp. This is beleived to 
be an issue that affected legacy versions of the software and has not been fixed in this release.

3.) Alert Response messages are not logged to the SQL database. This is beleived to be an issue that 
affected legacy versions of the software and has not been fixed in this release.

## License
Except where noted otherwise, the SAPIENT Middleware and Test Harness software is licensed under the Apache License, Version 2.0. Please see [License](LICENSE.txt) for details.

## Warranty and Support
The SAPIENT Test Harness software is supplied "as-is" without warranties or conditions of any kind.
The supplier is unable to offer support for the software.

