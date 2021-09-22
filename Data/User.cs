﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PMS.UserManagementAPI.Data
{
    [Index(nameof(Email), Name = "Email", IsUnique = true)]
    public partial class User
    {
        public User()
        {
            AppointmentPatients = new HashSet<Appointment>();
            AppointmentPhysicians = new HashSet<Appointment>();
            PatientDetails = new HashSet<PatientDetail>();
        }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(200)]
        public string LastName { get; set; }
        [Required]
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Phone { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime Dob { get; set; }
        [StringLength(100)]
        public string EmployeeId { get; set; }
        [Required]
        [StringLength(256)]
        public string Password { get; set; }
        public int NoOfWrongAttempts { get; set; }
        public int? RoleId { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsFirstTimeUser { get; set; }
        [Column("gender")]
        [StringLength(10)]
        public string Gender { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(UserRole.Users))]
        public virtual UserRole Role { get; set; }
        [InverseProperty(nameof(Appointment.Patient))]
        public virtual ICollection<Appointment> AppointmentPatients { get; set; }
        [InverseProperty(nameof(Appointment.Physician))]
        public virtual ICollection<Appointment> AppointmentPhysicians { get; set; }
        [InverseProperty(nameof(PatientDetail.User))]
        public virtual ICollection<PatientDetail> PatientDetails { get; set; }
    }
}
