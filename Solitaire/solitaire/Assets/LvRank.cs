﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LvRank  {
   static readonly int[] LvRanks = new int[] { 201,201, 507, 710, 827, 233, 415, 503, 306, 278, 285, 329, 609, 105, 222, 676, 135, 716, 595, 220, 754, 466, 361, 538, 243, 804, 667, 200, 206, 203, 302, 776, 190, 331, 805, 396, 559, 870, 880, 192, 120, 34, 258, 562, 577, 110, 134, 576, 179, 188, 817, 847, 633, 696, 40, 438, 53, 227, 205, 156, 33, 543, 547, 527, 395, 572, 504, 680, 764, 408, 607, 43, 207, 638, 618, 21, 263, 679, 231, 25, 346, 568, 579, 701, 796, 199, 184, 322, 74, 353, 257, 305, 844, 312, 86, 703, 432, 685, 176, 261, 240, 515, 587, 405, 483, 51, 812, 69, 99, 449, 417, 55, 826, 107, 268, 54, 411, 803, 250, 368, 871, 111, 180, 451, 669, 698, 75, 410, 472, 758, 24, 247, 794, 463, 617, 622, 610, 740, 88, 643, 150, 867, 628, 159, 13, 256, 642, 304, 239, 81, 202, 323, 377, 169, 277, 694, 363, 248, 221, 540, 860, 315, 591, 383, 330, 879, 8, 390, 186, 482, 132, 235, 303, 380, 198, 661, 68, 116, 388, 125, 326, 501, 820, 108, 97, 264, 292, 711, 31, 44, 7, 80, 389, 569, 737, 759, 811, 693, 829, 290, 445, 61, 372, 433, 840, 241, 384, 855, 780, 260, 314, 830, 496, 732, 165, 36, 382, 726, 22, 102, 866, 259, 557, 364, 727, 158, 126, 828, 308, 148, 66, 806, 224, 392, 479, 789, 506, 100, 131, 5, 223, 784, 214, 242, 84, 79, 157, 431, 45, 236, 663, 155, 634, 109, 767, 599, 781, 724, 800, 823, 373, 581, 3, 605, 735, 141, 251, 668, 209, 76, 47, 216, 4, 140, 469, 843, 474, 497, 27, 831, 163, 355, 85, 553, 775, 197, 467, 160, 19, 101, 178, 807, 41, 143, 271, 409, 119, 267, 535, 839, 601, 204, 654, 359, 675, 187, 38, 425, 57, 115, 136, 210, 347, 423, 399, 230, 273, 639, 193, 195, 673, 561, 858, 174, 262, 212, 376, 791, 145, 682, 67, 700, 265, 246, 374, 522, 505, 658, 600, 741, 768, 181, 164, 91, 82, 635, 130, 327, 436, 575, 117, 665, 446, 237, 18, 58, 20, 723, 795, 173, 344, 402, 437, 749, 832, 275, 60, 779, 809, 189, 534, 42, 455, 671, 72, 279, 398, 369, 300, 729, 325, 539, 706, 35, 697, 50, 788, 138, 2, 486, 270, 695, 786, 769, 29, 59, 367, 753, 349, 590, 215, 282, 783, 366, 461, 647, 336, 484, 293, 229, 873, 720, 46, 450, 674, 381, 298, 651, 755, 709, 815, 191, 32, 139, 777, 238, 354, 365, 473, 228, 64, 687, 509, 83, 244, 281, 481, 662, 851, 485, 859, 424, 611, 756, 418, 616, 333, 335, 443, 52, 760, 348, 412, 12, 458, 356, 171, 172, 26, 630, 166, 846, 378, 339, 714, 718, 770, 11, 489, 9, 253, 316, 686, 299, 92, 153, 358, 856, 338, 147, 648, 742, 151, 529, 232, 301, 319, 664, 825, 702, 133, 48, 15, 477, 743, 646, 296, 94, 295, 307, 70, 118, 28, 808, 841, 854, 167, 589, 772, 152, 488, 183, 340, 736, 23, 765, 332, 254, 73, 104, 106, 566, 337, 593, 249, 746, 692, 574, 681, 554, 787, 98, 328, 370, 17, 185, 666, 93, 498, 816, 397, 655, 785, 849, 350, 585, 715, 144, 352, 549, 269, 234, 311, 563, 442, 252, 385, 556, 375, 734, 37, 570, 362, 14, 30, 128, 536, 869, 211, 621, 637, 514, 518, 129, 434, 627, 394, 71, 196, 725, 168, 545, 121, 89, 266, 407, 430, 127, 532, 531, 124, 39, 491, 790, 602, 345, 875, 297, 177, 194, 650, 876, 146, 313, 560, 564, 636, 288, 457, 493, 511, 96, 555, 551, 341, 170, 793, 573, 56, 310, 161, 548, 583, 495, 541, 421, 626, 287, 645, 857, 456, 475, 704, 774, 6, 342, 317, 801, 717, 620, 175, 533, 103, 766, 291, 49, 528, 16, 613, 802, 113, 524, 114, 427, 677, 833, 419, 580, 77, 470, 226, 182, 750, 78, 519, 142, 406, 819, 810, 721, 728, 245, 623, 440, 712, 112, 747, 773, 510, 792, 90, 462, 868, 441, 567, 644, 225, 517, 276, 318, 386, 558, 722, 739, 162, 217, 707, 95, 631, 87, 429, 62, 842, 137, 584, 343, 65, 838, 274, 460, 10, 487, 521, 818, 689, 544, 608, 149, 404, 400, 255, 582, 597, 705, 63, 578, 219, 391, 452, 513, 294, 500, 272, 490, 835, 659, 730, 691, 782, 280, 218, 653, 799, 625, 439, 284, 122, 478, 745, 672, 850, 530, 387, 542, 546, 612, 688, 526, 778, 464, 762, 360, 861, 123, 154, 334, 508, 416, 874, 537, 731, 813, 401, 492, 834, 719, 757, 614, 320, 690, 751, 565, 598, 678, 821, 324, 512, 853, 420, 660, 814, 657, 683, 615, 752, 877, 586, 748, 480, 604, 640, 652, 619, 649, 403, 552, 459, 872, 379, 444, 865, 763, 836, 713, 213, 208, 771, 357, 699, 525, 428, 523, 864, 422, 629, 845, 351, 414, 797, 571, 588, 641, 447, 761, 453, 471, 594, 798, 837, 371, 448, 454, 502, 520, 670, 321, 606, 624, 656, 708, 822, 863, 426, 476, 603, 632, 852, 289, 848, 413, 309, 550, 468, 592, 744, 596, 733, 283, 286, 435, 516, 878, 393, 499, 684, 494, 465, 862, 824, 738 };
   public static int GetLv(int index)
    {
        int length = LvRanks.Length - 1;
        return LvRanks[index % length];
    }


}