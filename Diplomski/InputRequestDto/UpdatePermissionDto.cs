﻿namespace Diplomski.InputRequestDto
{
    public class UpdatePermissionDto
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;
    }
}
