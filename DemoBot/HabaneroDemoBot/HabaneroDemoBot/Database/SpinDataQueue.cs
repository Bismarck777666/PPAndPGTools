using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabaneroDemoBot.Database
{
    public class SpinDataQueue
    {
        private static SpinDataQueue _sInstance = new SpinDataQueue();
        
        public static SpinDataQueue Instance
        {
            get
            {
                return _sInstance;
            }
        }
        
        public List<SpinResponse> _responseQueue = new List<SpinResponse>();
        
        public void insertSpinDataToQueue(SpinResponse response)
        {
            lock (_responseQueue)
            {
                _responseQueue.Add(response);
            }
        }
        public void PushSpinDataItems(List<SpinResponse> items)
        {
            _responseQueue.InsertRange(0,items);
        }
        public List<SpinResponse> PopSpinDataItems(int count = 100)
        {
            List<SpinResponse> items = new List<SpinResponse>();

            if(_responseQueue.Count < 100)
            {
                return null;
            }
            if (_responseQueue.Count < count)
                count = _responseQueue.Count;

            if (count == 0)
                return null;

            items.AddRange(_responseQueue.GetRange(0, count));
            _responseQueue.RemoveRange(0, items.Count);
            return items;
        }
    }
}
