using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;


namespace BDBGlovesClient
{
    //Base Class for Connecting to tinyTILE
    public class BLEHandlingDiscovery : INotifyPropertyChanged, IDisposable
    {
        public BDBGCForm bdbgcform;
        BluetoothLEDevice bluetoothLEDevice;
        DeviceInformation MyDevice;
        DeviceWatcher deviceWatcher;

        static string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.ItemNameDisplay" };

        public BLEHandlingDiscovery(string bleDeviceName)
        {
            deviceWatcher =
                DeviceInformation.CreateWatcher(
                        bleDeviceName, //Accepts only devices named "BDB" or "Papageorgio" to avoid unwanted connections
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint); 
        }


        public void StartBLEDeviceWatcher()
        {
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;

            deviceWatcher.Start();

            Debug.WriteLine("StartBLEDeviceWatcher");
        }


        public void StopBLEDeviceWatcher()
        {
            deviceWatcher.Stop();
        }


        public Enum GetStatus()
        {
            return deviceWatcher.Status;
        }


        public BluetoothConnectionStatus GetConnectionStatus()
        {
            return bluetoothLEDevice.ConnectionStatus;
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            //Write Id and Name (Helps ensure visibility)
            Debug.WriteLine($"deviceInfo.Id: {deviceInfo.Id}");
            Debug.WriteLine($"deviceInfo.Name: {deviceInfo.Name}");

            //Find BLE Device
            MyDevice = deviceInfo;
            bluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            Debug.WriteLine($"Device Connected?: {bluetoothLEDevice.ConnectionStatus}");
            Debug.WriteLine($"Category: {bluetoothLEDevice.Appearance.Category}");
            Debug.WriteLine($"SubCategory: {bluetoothLEDevice.Appearance.SubCategory}");
            Debug.WriteLine($"RawValue: {bluetoothLEDevice.Appearance.RawValue}");
            Debug.WriteLine($"Address: {bluetoothLEDevice.BluetoothAddress}");
            Debug.WriteLine($"Address Type: {bluetoothLEDevice.BluetoothAddressType}");
            Debug.WriteLine($"Device Id: {bluetoothLEDevice.DeviceId}");
            Debug.WriteLine($"Is Device Pairable?: {bluetoothLEDevice.DeviceInformation.Pairing.CanPair}");
            Debug.WriteLine($"Pairing Protection Level?: {bluetoothLEDevice.DeviceInformation.Pairing.ProtectionLevel}");
            Debug.WriteLine($"Custom Pairing?: {bluetoothLEDevice.DeviceInformation.Pairing.Custom}");

            //Print and Add device services
            var bleServices = await bluetoothLEDevice.GetGattServicesAsync();
            foreach (var service in bleServices.Services)
            {
                Debug.WriteLine($"Service: {service.Uuid}");
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var character in characteristics.Characteristics)
                {
                    Debug.WriteLine($"Characteristic: {character.Uuid}");
                }
            }

            int connectionStatus = (int)bluetoothLEDevice.ConnectionStatus;

            if (connectionStatus == 1)
            {
                StartReceivingData();
            }
        }


        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {

        }


        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            int connectionStatus = (int)bluetoothLEDevice.ConnectionStatus;
            if (connectionStatus == 0)
            {
                bluetoothLEDevice.Dispose(); //Dispose if connection is lost
            }
        }


        #region Receive IMU Data
        public async void StartReceivingData()
        {
            //Print and Add device services
            var bleServices = await bluetoothLEDevice.GetGattServicesAsync();
            foreach (var service in bleServices.Services)
            {
                switch (service.Uuid.ToString()) 
                {
                    case SensorUUIDs.UUID_ACC_SERV:
                        InitializeAccelerationSensor(service);
                        InitializeGyroscopeSensor(service);
                        break;
                }
            }
        }


        private AccelerationSensor accSensor = null;
        protected /*async*/ void InitializeAccelerationSensor(GattDeviceService service)
        {
            accSensor = new AccelerationSensor(service);
            //await accSensor.EnableNotifications();
        }


        private GyroscopeSensor gyroSensor = null;
        protected /*async*/ void InitializeGyroscopeSensor(GattDeviceService service)
        {
            gyroSensor = new GyroscopeSensor(service);
            //await accSensor.EnableNotifications(); 
        }


        public float accAngleX { get; private set; }

        //public double accAngleY
        //{
        //    get { return accY; }
        //}

        //public double accAngleZ
        //{
        //    get { return accZ; }
        //}

        //public double gyroAngleX
        //{
        //    get { return gyroX; }
        //}

        public float gyroAngleY { get; private set; }

        public object ConnectionStatus { get; internal set; }

        //public double gyroAngleZ
        //{
        //    get { return gyroZ; }
        //}

        public async void UpdateAllData()
        {
            if (accSensor != null)
            {
                var accData = await accSensor.GetAcceleration();

                accAngleX = accData;
                //accY = accData[1];
                //accZ = accData[2];

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("accAngleX"));
            }
            if (gyroSensor != null) 
            {
                var gyroData = await gyroSensor.GetAngularVelocity();

                //gyroX = gyroData[0];
                  gyroAngleY = gyroData;
                //gyroZ = gyroData[2];

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("gyroAngleY"));
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        public void Dispose()
        {
            //StopReceivingData();
            if (accSensor != null) accSensor.Dispose();
            if (gyroSensor != null) gyroSensor.Dispose();
        }
    }



    //Class containing Uuids of tinyTILE sensors
    public static class SensorUUIDs
    {
        //Acc Service UUID
        public const string UUID_ACC_SERV = "0000180f-0000-1000-8000-00805f9b34fb";

        //Acceleration Sensor UUID
        public const string UUID_ACC_DATA = "00002a19-0000-1000-8000-00805f9b34fb";

        //Gyroscope Sensor UUID
        public const string UUID_GYRO_DATA = "00002a1a-0000-1000-8000-00805f9b34fb";
    }



    //Base Class for reading data from sensors
    public class SensorBase : IDisposable
    {
        protected GattDeviceService deviceService;
        protected string sensorDataUuid;
        protected byte[] data;
        protected bool isNotificationSupported = false;
        private GattCharacteristic dataCharacteristic;

        public SensorBase(GattDeviceService dataService, string sensorDataUuid)
        {
            this.deviceService = dataService;
            this.sensorDataUuid = sensorDataUuid;
        }


        public virtual async Task EnableNotifications()
        {
            isNotificationSupported = true;
            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];
            dataCharacteristic.ValueChanged += dataCharacteristic_ValueChanged;
            GattCommunicationStatus status = 
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
        }


        public virtual async Task DisableNotifications()
        {
            isNotificationSupported = false;
            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];
            dataCharacteristic.ValueChanged -= dataCharacteristic_ValueChanged;
            GattCommunicationStatus status =
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.None);
        }


        protected async Task<byte[]> ReadValue() 
        {
            if (!isNotificationSupported) 
            {
                if (dataCharacteristic == null)
                    dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                        new Guid(sensorDataUuid))).Characteristics[0]; 
                GattReadResult readResult =
                    await dataCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                data = new byte[readResult.Value.Length];
                DataReader.FromBuffer(readResult.Value).ReadBytes(data);
            }
            return data;
        }


        private void dataCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
        }


        public async void Dispose()
        {
             await DisableNotifications();
       
        }
    }



    public class AccelerationSensor : SensorBase
    {
        public AccelerationSensor(GattDeviceService service) : base(service, SensorUUIDs.UUID_ACC_DATA) {}

        public async Task<float> GetAcceleration()
        {
            float accData;
            var accByteData = await base.ReadValue();

            //float[] accData = new float[3];
            //var accByteData = await base.ReadValue();

            accData = BitConverter.ToSingle(accByteData, 0);
            //accData[1] = BitConverter.ToSingle(accByteData, 4);
            //accData[2] = BitConverter.ToSingle(accByteData, 8);

            return accData;
        }
    }



    public class GyroscopeSensor : SensorBase
    {
        public GyroscopeSensor(GattDeviceService service) : base(service, SensorUUIDs.UUID_GYRO_DATA) {}

        public async Task<float> GetAngularVelocity()
        {
            
            float gyroData;
            var gyroByteData = await base.ReadValue();

            gyroData = BitConverter.ToSingle(gyroByteData, 0);
            //gyroData[1] = BitConverter.ToSingle(gyroByteData, 4);
            //gyroData[2] = BitConverter.ToSingle(gyroByteData, 8);

            return gyroData;
        }
    }
    #endregion
}
