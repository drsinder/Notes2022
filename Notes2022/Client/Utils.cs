//using Notes2022.Shared;
//using Syncfusion.Blazor.Grids;
//using Syncfusion.Blazor.Inputs;
//using System.Net.Http.Json;

//using Blazored.Modal.Services;
//using Blazored.Modal;
//namespace Notes2022.Client
//{
//    public static class Utils
//    {

//        public static string ParseAndNavigate(int fileId, string myString)
//        {
//            int noteNum = 0;
//            int noteRespOrd = 0;

//            // parse string for # or #.#
//            string stuff = myString;

//            stuff = stuff.Replace(";", "").Replace(" ", "");
//            string[] parts = stuff.Split('.');
//            if (parts.Length > 2)
//            {
//                ShowMessage("Too many '.'s : " + parts.Length);
//            }
//            if (parts.Length == 1)
//            {
//                if (!int.TryParse(parts[0], out noteNum))
//                {
//                    ShowMessage("Could not parse : " + parts[0]);
//                }
//                else
//                {
//                    LongWrapper wrapper = await Http.GetFromJsonAsync<LongWrapper>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/0");
//                    long headerId = wrapper.mylong;
//                    if (headerId != 0)
//                        Navigation.NavigateTo("notedisplay/" + headerId);
//                    else
//                        ShowMessage("Could not find note : " + stuff);

//                }
//            }
//            else if (parts.Length == 2)
//            {
//                if (!int.TryParse(parts[0], out noteNum))
//                {
//                    ShowMessage("Could not parse : " + parts[0]);
//                }
//                if (!int.TryParse(parts[1], out noteRespOrd))
//                {
//                    ShowMessage("Could not parse : " + parts[1]);
//                }
//                if (noteNum != 0 && noteRespOrd != 0)
//                {
//                    LongWrapper wrapper = await Http.GetFromJsonAsync<LongWrapper>("api/GetNoteHeaderId/" + NotesfileId + "/" + noteNum + "/" + noteRespOrd);
//                    long headerId = wrapper.mylong;
//                    if (headerId != 0)
//                        Navigation.NavigateTo("notedisplay/" + headerId);
//                    else
//                        ShowMessage("Could not find note : " + stuff);
//                }
//            }
//            return "";
//        }

//    }
//}
