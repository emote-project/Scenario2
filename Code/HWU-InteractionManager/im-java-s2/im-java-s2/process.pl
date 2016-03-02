


open (IN, "gameLog1.txt");
open (OUT, ">temp.txt");
while(<IN>){
	if (m/cellExamined/){
		next;
	}
	if (! m/^{/){
		next;
	}
	print OUT;
}
close (IN);