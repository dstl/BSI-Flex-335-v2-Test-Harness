// Crown-owned copyright, 2021-2024
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Sapient.Common;
using Sapient.Data;
using SapientDatabase;

namespace SapientServices.Tests
{
    /// <summary>
    /// The base tests.
    /// </summary>
    public abstract class BaseTests
    {
        protected Database sapientDatabase;
        private string databaseServer = "127.0.0.1";
        private string databasePort = "5432";
        private string databaseName = "sapientV3";
        private string databaseUser = "postgres";
        private string databasePassword = "password";

        /// <summary>
        /// Ins the it.
        /// </summary>
        [SetUp]
        public void InIt()
        {
            sapientDatabase = new Database(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
        }

        /// <summary>
        /// Cleanups the.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            sapientDatabase.Close();
        }

        public Location DefaultLocation
        {
            get
            {
                return new Location
                {
                    X = 50.00,
                    Y = 1.36,
                    Z = 100,
                    XError = 2.00,
                    YError = 1.00,
                    ZError = 3.00,
                    CoordinateSystem = LocationCoordinateSystem.Latlngdegm,
                    Datum = LocationDatum.Wgs84G,
                };
            }
        }

        public RangeBearing DefaultRangeBearing
        {
            get
            {
                return new RangeBearing
                {
                    Elevation = 20.00,
                    Azimuth = 10.05,
                    Range = 200.00,
                    ElevationError = 1.00,
                    AzimuthError = 2.00,
                    RangeError = 0.02,
                    CoordinateSystem = RangeBearingCoordinateSystem.Degreesm,
                    Datum = RangeBearingDatum.Platform,
                };
            }
        }

        public RangeBearingCone DefaultRangeBearingCone
        {
            get
            {
                return new RangeBearingCone
                {
                    Elevation = 20.00,
                    Azimuth = 10.05,
                    Range = 200.00,
                    HorizontalExtent = 10.00,
                    VerticalExtent = 0.20,
                    VerticalExtentError = 3.00,
                    HorizontalExtentError = 0.20,
                    ElevationError = 1.00,
                    AzimuthError = 2.00,
                    RangeError = 0.02,
                    CoordinateSystem = RangeBearingCoordinateSystem.Degreesm,
                    Datum = RangeBearingDatum.Platform,
                };
            }
        }

        public Registration.Types.Duration DefaultDuration
        {
            get
            {
                return new Registration.Types.Duration
                {
                    Units = Registration.Types.TimeUnits.Seconds,
                    Value = 5,
                };
            }
        }

        public Registration.Types.LocationType DefaultLocationTypePoint
        {
            get
            {
                return new Registration.Types.LocationType
                {
                    LocationUnits = LocationCoordinateSystem.Latlngdegm,
                    LocationDatum = LocationDatum.Wgs84G,
                };
            }
        }

        public Registration.Types.LocationType DefaultLocationTypeStrobe
        {
            get
            {
                return new Registration.Types.LocationType
                {
                    RangeBearingUnits = RangeBearingCoordinateSystem.Degreesm,
                    RangeBearingDatum = RangeBearingDatum.True,
                };
            }
        }

        public AssociatedFile DefaultAssociatedFile
        {
            get
            {
                return new AssociatedFile
                {
                    Type = "Detection Image",
                    Url = "https://democloud.co.uk/shared/alert.jpg",
                };
            }
        }

        public AssociatedDetection DefaultAssociatedDetection
        {
            get
            {
                return new AssociatedDetection
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    NodeId = Ulid.NewUlid().ToString(),
                    ObjectId = Ulid.NewUlid().ToString(),
                    Description = "Associated Detection details.",
                    Location = DefaultLocation,
                };
            }
        }
    }
}
