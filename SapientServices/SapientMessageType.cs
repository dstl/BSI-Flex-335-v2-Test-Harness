// File:              $Workfile: SapientMessageType.cs$
// <copyright file="SapientMessageType.cs" >
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// </copyright>

namespace SapientServices
{
    /// <summary>
    /// The SapientMessageType enum.
    /// </summary>
    public enum SapientMessageType
    {
        Detection = 0,
        Status = 1,
        Alert = 2,
        Task = 3,
        TaskACK = 4,
        AlertResponse = 5,
        Registration = 6,
        IdError = 7,
        InternalError = 8,
        InvalidTasking = 9,
        Unknown = 10,
        InvalidClient = 11,
        RegistrationACK = 12,
        Error = 13,
        ResponseIdError = 14,
        Unsupported = 15,
        Reserved = 16,
        Objective = 17,
        RoutePlan = 18,
        Approval = 19,
        SensorTaskDropped = 20,
        SensorTaskTakeControl = 21,
        SensorTaskReleaseControl = 22
    }
}
