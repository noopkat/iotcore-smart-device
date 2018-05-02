namespace SmartDevice
{
    /// <summary>
    /// the registers we'll be using. note: this is not the full set of registers available
    /// </summary>
    public static class RegistersConstants
    {
        public const byte Command = 0x80;
        public const byte ProductId = 0x81;
        public const byte MeasureProximity = 0x08;
        public const byte MeasureProximityReady = 0x20;
        public const byte MeasureProximityData = 0x87;
        public const byte SetProximityIrLed = 0x83;
        public const byte SetProximityReadRate = 0x8f;
        public const byte SetInterruptThreshold = 0x89;
        public const byte SetInterruptHighByte = 0x8C;
        public const byte SetInterruptLowByte = 0x8D;
        public const byte SetInterruptFlag = 0x8e;
    }
}