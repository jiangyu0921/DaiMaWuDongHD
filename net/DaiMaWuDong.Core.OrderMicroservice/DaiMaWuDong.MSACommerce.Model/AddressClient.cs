using DaiMaWuDong.MSACommerce.DTOModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Model
{
    public class AddressClient
    {
        public static readonly List<AddressDTO> addressList = new List<AddressDTO>()
        {new  AddressDTO(){
        id=1L,
         address="济南市历下区************",
           city="济南",
            district="历下区",
            name="gerry",
            phone="15855500000",
            state="武汉",
        zipCode="100010",
        isDefault=true
        },
        new  AddressDTO()
        {
                id=1L,
         address="济南市历下区************",
           city="济南",
            district="历下区",
            name="gerry",
            phone="1234569877",
            state="济南",
        zipCode="100010",
        isDefault=true
    }
};

        public static AddressDTO FindById(long id)
        {
            foreach (AddressDTO addressDTO in addressList)
            {
                if (addressDTO.id == id) return addressDTO;
            }
            return null;
        }
    }
}
