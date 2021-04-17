using HCaptcha_Solver.JSON;

public class HCaptchaGetCaptcha
{
    public C c { get; set; }
    public string challenge_uri { get; set; }
    public string key { get; set; }
    public Request_Config request_config { get; set; }
    public string request_type { get; set; }
    public Requester_Question requester_question { get; set; }
    public string[] requester_question_example { get; set; }
    public Tasklist[] tasklist { get; set; }
    public string bypassmessage { get; set; }
}

public class Request_Config
{
    public int version { get; set; }
    public object shape_type { get; set; }
    public object min_points { get; set; }
    public object max_points { get; set; }
    public object min_shapes_per_image { get; set; }
    public object max_shapes_per_image { get; set; }
    public object restrict_to_coords { get; set; }
    public object minimum_selection_area_per_shape { get; set; }
    public int multiple_choice_max_choices { get; set; }
    public int multiple_choice_min_choices { get; set; }
}

public class Requester_Question
{
    public string en { get; set; }
}

public class Tasklist
{
    public string datapoint_uri { get; set; }
    public string task_key { get; set; }
}
