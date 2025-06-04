using UnityEngine;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    public List<CustomerData> customerList = new List<CustomerData>();

    public CustomerData GetCustomerByName(string name)
    {
        return customerList.Find(c => c.customerName == name);
    }

    public CustomerData GetRandomCustomer()
    {
        if (customerList.Count == 0) return null;
        return customerList[Random.Range(0, customerList.Count)];
    }
}
